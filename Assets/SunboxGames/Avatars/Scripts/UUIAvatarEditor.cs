
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
using SQLiter;
using Player;

namespace Sunbox.Avatars {

    public class UUIAvatarEditor : MonoBehaviour {

        const string CONTENT_STRING = " (Content)";
        const string CONTENT_STRING_HIDE = " (Content Hide)";

        public AvatarCustomization Avatar;
        
        public Camera Camera;

        [Space(20)]
        public Button MaleButton;
        public Button FemaleButton;

        [Space(20)]
        public Button FaceButton;
        public Button ClothingButton;

        public Button FinishButton;

        [Space(20)]
        public GameObject SliderTemplate;
        public GameObject TitleTemplate;
        public GameObject ClothingItemDropdownTemplate;
        public GameObject Canvas;
        public GameObject SidePanel;

        public GameObject SignUpPanel;
        public GameObject LoginPanel;

        [Space(20)]
        public RectTransform ContentTransform;

        private Vector3 _cameraPosition;
        private Vector3 _cameraRotation;
        private Vector3 _enlargedCameraPosition = new Vector3(5,1,10);
        private Vector3 _enlargedCameraRotation = new Vector3(8,180,0);

        private ClothingDropdownWrapper _hatClothingDropdown;
        private ClothingDropdownWrapper _topClothingDropdown;
        private ClothingDropdownWrapper _bottomClothingDropdown;
        private ClothingDropdownWrapper _shoesClothingDropdown;
        private ClothingDropdownWrapper _glassesClothingDropdown;

        private UClothingItem _hiddenGlasses;
        
        private List<SliderWrapper> _sliders = new List<SliderWrapper>();
        private static Dictionary<string, int> clothData = new Dictionary<string, int>();



        void Start() {
            _cameraPosition = Camera.transform.position;
            _cameraRotation = Camera.transform.eulerAngles;
            SidePanel.SetActive(false);
            SignUpPanel.SetActive(false);
            LoginPanel.SetActive(false);

            MaleButton.onClick.AddListener(() => Avatar.SetGender(AvatarCustomization.AvatarGender.Male, true));
            FemaleButton.onClick.AddListener(() => Avatar.SetGender(AvatarCustomization.AvatarGender.Female, true));

            FaceButton.onClick.AddListener(() => {
                ClearSectionContent_Internal();
                ShowSliderList_Internal(AvatarCustomization.SECTION_FACE_STYLES);

                _hiddenGlasses = Avatar.GetComponentsInChildren<UClothingItem>().FirstOrDefault(item => item.ClothingItem.SlotType == SlotType.Glasses);
                if (_hiddenGlasses != null) {
                    _hiddenGlasses.gameObject.SetActive(false);
                }
            });
            ClothingButton.onClick.AddListener(() => {
                ClearSectionContent_Internal();
                ShowClothingList_Internal();

                if (_hiddenGlasses != null) {
                    _hiddenGlasses.gameObject.SetActive(true);
                }
            });
            SliderTemplate.SetActive(false);
            TitleTemplate.SetActive(false);
            ClothingItemDropdownTemplate.SetActive(false);
            FinishButton.onClick.AddListener(() => {
                Canvas.SetActive(false);
                SidePanel.SetActive(true);
                Camera.transform.position = _enlargedCameraPosition;
                Camera.transform.rotation = Quaternion.Euler(_enlargedCameraRotation);
                Debug.Log(AvatarCustomization.ToConfigString(AvatarCustomization.Instance));
                SQLiter.SQLite.Instance.InsertClothInfo(Player.LoginController.playerId, clothData);
            });
        }

        void Update() {
        }

        private void UpdateSliderList_Internal() {
            if (_sliders == null) {
                return;
            }

            FieldInfo[] fields = typeof(AvatarCustomization).GetFields();

            foreach (FieldInfo field in fields) {
                AvatarFieldAttribute avatarFieldAttribute = field.GetCustomAttribute<AvatarFieldAttribute>();
                if (avatarFieldAttribute == null) {
                    continue;
                }
                
                SliderWrapper slider = _sliders.FirstOrDefault(slider => slider.FieldName == field.Name);
                if (slider == null) {
                    continue;
                }

                slider.CurrentValue = slider.WholeNumbers ? (int) field.GetValue(Avatar) : (float) field.GetValue(Avatar);
            }
        }

        private void ShowSliderList_Internal(string section) {
            FieldInfo[] fields = typeof(AvatarCustomization).GetFields();

            foreach (FieldInfo field in fields) {
                AvatarFieldAttribute avatarFieldAttribute = field.GetCustomAttribute<AvatarFieldAttribute>();
                if (avatarFieldAttribute == null || avatarFieldAttribute.Section != section) {
                    continue;
                }

                AddSlider_Internal(field, fields);
            }

            SliderTemplate.SetActive(false);
            TitleTemplate.SetActive(false);
        }

        private void ClearSectionContent_Internal() {
            foreach (Transform transform in ContentTransform) {
                if (transform.name.Contains(CONTENT_STRING)) {
                    Destroy(transform.gameObject);
                }
            }

            foreach (Transform transform in ContentTransform) {
                if (transform.name.Contains(CONTENT_STRING_HIDE)) {
                    transform.gameObject.SetActive(false);
                }
            }

            _sliders.Clear();
        }

        private void ShowClothingList_Internal() {

            if (_hatClothingDropdown == null) {
                _hatClothingDropdown = new ClothingDropdownWrapper(ClothingItemDropdownTemplate, Avatar, SlotType.Hat).AttachTo(ContentTransform);
                _hatClothingDropdown.Title = "Hat";
            }
            else {
                _hatClothingDropdown.Toggle(true);
            }

            if (_topClothingDropdown == null) {
                _topClothingDropdown = new ClothingDropdownWrapper(ClothingItemDropdownTemplate, Avatar, SlotType.Top).AttachTo(ContentTransform);
                _topClothingDropdown.Title = "Torso/Top";
            }
            else {
                _topClothingDropdown.Toggle(true);
            }
            
            if (_bottomClothingDropdown == null) {
                _bottomClothingDropdown = new ClothingDropdownWrapper(ClothingItemDropdownTemplate, Avatar, SlotType.Bottom).AttachTo(ContentTransform);
                _bottomClothingDropdown.Title = "Pants/Bottom";
            }
            else {
                _bottomClothingDropdown.Toggle(true);
            }
            
            if (_glassesClothingDropdown == null) {
                _glassesClothingDropdown = new ClothingDropdownWrapper(ClothingItemDropdownTemplate, Avatar, SlotType.Glasses).AttachTo(ContentTransform);
                _glassesClothingDropdown.Title = "Glasses";
            }
            else {
                _glassesClothingDropdown.Toggle(true);
            }

            if (_shoesClothingDropdown == null) {
                _shoesClothingDropdown = new ClothingDropdownWrapper(ClothingItemDropdownTemplate, Avatar, SlotType.Shoes).AttachTo(ContentTransform);
                _shoesClothingDropdown.Title = "Shoes";
            }
            else {
                _shoesClothingDropdown.Toggle(true);
            }
        }

        private void AddSlider_Internal(FieldInfo field, FieldInfo[] fields) {
            AvatarFloatFieldAttribute floatFieldAttribute = field.GetCustomAttribute<AvatarFloatFieldAttribute>();
            if (floatFieldAttribute != null && !floatFieldAttribute.IgnoreInPlayMode) {
                SliderWrapper slider = new SliderWrapper(field.Name, SliderTemplate) {
                    Title = floatFieldAttribute.DisplayName,
                    CurrentValue = (float) field.GetValue(Avatar),
                    MinValue = floatFieldAttribute.SourceMinValue,
                    MaxValue = floatFieldAttribute.SourceMaxValue
                };
                
                slider.OnValueChanged = (value) => {
                    switch (floatFieldAttribute.DisplayName.ToLower()) 
                    {
                        case "skin":
                            clothData["skin"] = (int)value;
                            break;

                        case "eye material":
                            clothData["eye_material"] = (int)value;
                            break;

                        case "hair material":
                            clothData["hair_material"] = (int)value;
                            break;

                        case "brow material":
                            clothData["brow_material"] = (int)value;
                            break;

                        default:
                            Debug.LogWarning($"Unknown field: {field}");
                            break;
                    }
                    field.SetValue(Avatar, (float) value);
                    Avatar.UpdateCustomization();
                };

                slider.AttachTo(ContentTransform);

                _sliders.Add(slider);
            }

            AvatarIntegerFieldAttribute integerFieldAttribute = field.GetCustomAttribute<AvatarIntegerFieldAttribute>();
            if (integerFieldAttribute != null && integerFieldAttribute.HasArrayDependancy()) {
                
                // Get array
                FieldInfo[] referencesFieldInfos = Avatar.AvatarReferences.GetType().GetFields();
                Array dependancyArrayObject = (Array) referencesFieldInfos.First(field => field.Name == integerFieldAttribute.ArrayDependancyField).GetValue(Avatar.AvatarReferences);

                // Check if its variation field
                if (integerFieldAttribute.IsVariationField) {
                    IVariations variationsInterface = (IVariations) dependancyArrayObject.GetValue(0);
                    
                    SliderWrapper slider = new SliderWrapper(field.Name, SliderTemplate) {
                        CurrentValue = (int) field.GetValue(Avatar),
                        MinValue = 0,
                        MaxValue = variationsInterface.GetVariationsCount() - 1,
                        WholeNumbers = true
                    };

                    slider.OnValueChanged = (value) => {
                        field.SetValue(Avatar, (int) value);
                        Avatar.UpdateCustomization();
                        slider.Title = $"{integerFieldAttribute.DisplayName} ({variationsInterface.GetVariationName((int) value)})";
                    };

                    slider.AttachTo(ContentTransform);
                    slider.Title = $"{integerFieldAttribute.DisplayName} ({variationsInterface.GetVariationName((int) slider.CurrentValue)})";
                    _sliders.Add(slider);
                }
                else {
                    SliderWrapper slider = new SliderWrapper(field.Name, SliderTemplate) {
                        CurrentValue = (int) field.GetValue(Avatar),
                        MinValue = 0,
                        MaxValue = dependancyArrayObject.Length - 1,
                        WholeNumbers = true
                    };

                    slider.OnValueChanged = (value) => {
                        field.SetValue(Avatar, (int) value);
                        Avatar.UpdateCustomization();
                        slider.Title = $"{integerFieldAttribute.DisplayName} ({GetObjectName_Internal(dependancyArrayObject, (int) value)})";
                    };

                    slider.AttachTo(ContentTransform);
                    slider.Title = $"{integerFieldAttribute.DisplayName} ({GetObjectName_Internal(dependancyArrayObject, (int) slider.CurrentValue)})";
                    _sliders.Add(slider);
                }
            }
        }

        private string GetObjectName_Internal(Array array, int index) {
            object value = array.GetValue(index);
            if (value is Material) {
                return ((Material) value).name;
            }

            return value.ToString();
        }

        private void AddTitleText_Internal(string title) {
            GameObject titleInstance = Instantiate(TitleTemplate);
            titleInstance.transform.SetParent(ContentTransform, false);
            titleInstance.GetComponent<TextMeshProUGUI>().text = title;
        }

        public class ClothingDropdownWrapper {

            public string Title {
                get {
                    return _title;
                }
                set {
                    if (_instance != null) {
                        _instance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value;
                    }

                    _title = value;
                }
            }

            private string _title;
            private Slider _slider;
            private TMP_Dropdown _dropdown; 

            private GameObject _clothingDropdownTemplate;
            private GameObject _instance;

            private AvatarCustomization _avatar;
            private ClothingItem[] _availableClothingItems;
            private SlotType _slotType;

            private Action<ClothingItem> _onClothingItemSelected;

            private ClothingItem _selectedClothingItem;
            private int _selectedClothingItemIndex;

            private LayoutElement _layoutElement;

            private const float MIN_HEIGHT = 60;
            private const float MAX_HEIGHT = 100;

            public ClothingDropdownWrapper(GameObject ClothingDropdownTemplate, AvatarCustomization avatar, SlotType slotType) {
                _clothingDropdownTemplate = ClothingDropdownTemplate;
                _avatar = avatar;
                _slotType = slotType;

                _availableClothingItems = _avatar.AvatarReferences.AvailableClothingItems.Where(item => item.SlotType == _slotType).ToArray();
            }

            public ClothingDropdownWrapper AttachTo(Transform transform) {
                _instance = Instantiate(_clothingDropdownTemplate);
                _layoutElement = _instance.GetComponent<LayoutElement>();
                _dropdown = _instance.transform.GetChild(1).GetComponent<TMP_Dropdown>();
                _slider = _instance.transform.GetChild(2).GetComponent<Slider>();

                _selectedClothingItem = _avatar.GetClothingItemFromSlot(_slotType) != null ? _availableClothingItems.FirstOrDefault(item => item == _avatar.GetClothingItemFromSlot(_slotType)) : null;
                _selectedClothingItemIndex = _avatar.GetClothingItemVariationIndex(_slotType);

                UpdateSliderText_Internal(_selectedClothingItem != null ? _selectedClothingItem.Variations[_avatar.GetClothingItemVariationIndex(_slotType)].name : "");
                
                _dropdown.ClearOptions();
                List<string> list = _avatar.AvatarReferences.AvailableClothingItems.Where(item => item.SlotType == _slotType).Select(item => item.Name).ToList();
                _dropdown.AddOptions(list);
            
                _dropdown.onValueChanged.AddListener((index) => {
                    Debug.Log($"   cloth:{list[index]}");
                    if (list != null && _avatar.AvatarReferences.AvailableClothingItems != null && index >= 0 && index < list.Count)
                    {
                        List<string> AvailableClothinglist = _avatar.AvatarReferences.AvailableClothingItems.Where(item => item.SlotType == _slotType).Select(item => item.Name).ToList();
                        int indexInList = AvailableClothinglist.IndexOf(list[index]);

                        if (indexInList != -1)  // Ensure the item exists in the list
                        {
                            Debug.Log($"   cloth:{indexInList}");
                            switch (_slotType)
                            {
                                case SlotType.Hat:
                                    clothData["hat"] = indexInList;
                                    break;

                                case SlotType.Top:
                                    clothData["top"] = indexInList;
                                    break;

                                case SlotType.Bottom:
                                    clothData["bottom"] = indexInList;
                                    break;

                                case SlotType.Glasses:
                                    clothData["glasses"] = indexInList;
                                    break;

                                default:
                                    Debug.LogWarning($"Unknown field: {_slotType}");
                                    break;
                            }
                        }
                        else
                        {
                            Debug.LogError($"Item not found in AvailableClothingItems! index: {index}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Invalid index ({index}) or null list detected!");
                    }
                    
                    _selectedClothingItem = _availableClothingItems[index];
                    _selectedClothingItemIndex = index;

                    _avatar.SetClothingItemVariation(_slotType, 0);

                    _avatar.AttachClothingItem(_selectedClothingItem);
                    _avatar.UpdateClothing();

                    UpdateSlider_Internal();
                    UpdateSliderText_Internal(_selectedClothingItem.Variations[0].name);
                });

                _instance.transform.SetParent(transform, false);
                _instance.name += CONTENT_STRING_HIDE;
                _instance.SetActive(true);

                UpdateSlider_Internal();

                return this;
            }

            public void SetOnClothingItemSelected(Action<ClothingItem> onClothingItemSelected) {
                _onClothingItemSelected = onClothingItemSelected;
                if (_dropdown != null) {
                    _dropdown.onValueChanged.RemoveAllListeners();
                    _dropdown.onValueChanged.AddListener((index) => {
                        ClothingItem selectedClothingItem = _availableClothingItems[index];
                        _onClothingItemSelected.Invoke(selectedClothingItem);
                    });
                }
            }

            public void UpdateValues(bool notify = true) {
                ClothingItem clothingItem = _avatar.GetClothingItemFromSlot(_slotType);

                if (notify) {
                    _dropdown.value = clothingItem != null ? _availableClothingItems.FirstIndexMatch(item => item == clothingItem) : 0;
                    _slider.value = (clothingItem != null ? _avatar.GetClothingItemVariationIndex(_slotType) : 0);
                }
                else {
                    _dropdown.SetValueWithoutNotify(clothingItem != null ? _availableClothingItems.FirstIndexMatch(item => item == clothingItem) : 0);
                    _slider.SetValueWithoutNotify(clothingItem != null ? _avatar.GetClothingItemVariationIndex(_slotType) : 0);
                }

                _slider.gameObject.SetActive(clothingItem != null ? clothingItem.Variations.Length > 1 : false);
                _layoutElement.minHeight = clothingItem != null && clothingItem.Variations.Length > 1 ? MAX_HEIGHT : MIN_HEIGHT;

                Vector2 size = _instance.GetComponent<RectTransform>().sizeDelta;
                size.y = clothingItem != null && clothingItem.Variations.Length > 1 ? MAX_HEIGHT : MIN_HEIGHT;
                _instance.GetComponent<RectTransform>().sizeDelta = size;

                if (clothingItem != null) {
                    UpdateSliderText_Internal(clothingItem.Variations[_avatar.GetClothingItemVariationIndex(_slotType)].name);
                }
            }

            public void Toggle(bool visibility) {
                if (_instance != null) {
                    _instance.SetActive(visibility);
                }
            }

            private void UpdateSlider_Internal() {
                _slider.gameObject.SetActive(_selectedClothingItem != null ? _selectedClothingItem.Variations.Length > 1 : false);
                _layoutElement.minHeight = _selectedClothingItem != null && _selectedClothingItem.Variations.Length > 1 ? MAX_HEIGHT : MIN_HEIGHT;
                
                Vector2 size = _instance.GetComponent<RectTransform>().sizeDelta;
                size.y = _selectedClothingItem != null && _selectedClothingItem.Variations.Length > 1 ? MAX_HEIGHT : MIN_HEIGHT;
                _instance.GetComponent<RectTransform>().sizeDelta = size;
               
                _slider.minValue = 0;
                _slider.maxValue = _selectedClothingItem != null ? _selectedClothingItem.Variations.Length - 1 : 0;
                _slider.wholeNumbers = true;
                _slider.onValueChanged.RemoveAllListeners();
                _slider.onValueChanged.AddListener((value) => {
                    if (_slotType == SlotType.Top)
                    {
                        clothData["top_material"] = (int)value;
                    }
                    _avatar.SetClothingItemVariation(_slotType, (int) value);
                    _avatar.UpdateClothing();
                    UpdateSliderText_Internal(_selectedClothingItem.Variations[(int) value].name);
                });
            }
            
            private void UpdateSliderText_Internal(string text) {
                _slider.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
            }

            public void UpdateClothingItem(SlotType slotType, ClothingItem clothingItem) {
                if (_instance == null) {
                    Debug.Log($"Cannot update {nameof(ClothingDropdownWrapper)} since instance is null. Attach it first.");
                    return;    
                }

                _dropdown.value = _availableClothingItems.FirstIndexMatch(item => item == _avatar.GetClothingItemFromSlot(slotType));

            }

        }

        public class SliderWrapper {

            public string FieldName;
            
            public string Title {
                get {
                    return _title;
                }
                set {
                    _title = value;
                    if (_instantiatedSlider != null) {
                        _instantiatedSlider.transform.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().text = _title;
                    }
                }
            }

            public float MinValue { get; set; }

            public float MaxValue { get; set; }

            public float CurrentValue {
                get {
                    return _currentValue;
                }
                set {
                    _currentValue = value;
                    if (_slider != null) {
                        _slider.value = _currentValue;
                    }
                }
            }

            public UnityAction<float> OnValueChanged { 
                set {
                    _onValueChanged = value;
                } 
            }
            
            public bool WholeNumbers { get; set; } = false;

            private GameObject _sliderTemplate;
            private GameObject _instantiatedSlider;
            private Slider _slider;
            private UnityAction<float> _onValueChanged;
            private string _title;
            private float _currentValue;

            public SliderWrapper(string fieldName, GameObject SliderTemplate) {
                _sliderTemplate = SliderTemplate;
                FieldName = fieldName;
            } 

            public void AttachTo(Transform transform) {
                _instantiatedSlider = Instantiate(_sliderTemplate);
                _instantiatedSlider.transform.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().text = Title;

                _slider = _instantiatedSlider.transform.GetChild(0).GetComponent<Slider>();
                _slider.wholeNumbers = WholeNumbers;
                _slider.minValue = MinValue;
                _slider.maxValue = MaxValue;
                _slider.value = _currentValue;
                _slider.onValueChanged.RemoveAllListeners();
                _slider.onValueChanged.AddListener((value) => {
                    _currentValue = value;
                    _onValueChanged.Invoke(value);
                });
                _instantiatedSlider.transform.SetParent(transform, false);
                _instantiatedSlider.name += CONTENT_STRING;
                _instantiatedSlider.SetActive(true);
            }
        }
    }
}
