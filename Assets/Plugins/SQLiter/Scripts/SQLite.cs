//----------------------------------------------
// SQLiter
// Copyright � 2014 OuijaPaw Games LLC
//----------------------------------------------

using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;
using System;
using System.Collections.Generic;


namespace SQLiter
{
	/// <summary>
	/// The idea is that here is a bunch of the basics on using SQLite
	/// Nothing is some advanced course on doing joins and unions and trying to make your infinitely normalized schema work
	/// SQLite is simple.  Very simple.  
	/// Pros:
	/// - Very simple to use
	/// - Very small memory footprint
	/// 
	/// Cons:
	/// - It is a flat file database.  You can change the settings to make it run completely in memory, which will make it even
	/// faster; however, you cannot have separate threads interact with it -ever-, so if you plan on using SQLite for any sort
	/// of multiplayer game and want different Unity instances to interact/read data... they absolutely cannot.
	/// - Doesn't offer as many bells and whistles as other DB systems
	/// - It is awfully slow.  I mean dreadfully slow.  I know "slow" is a relative term, but unless the DB is all in memory, every
	/// time you do a write/delete/update/replace, it has to write to a physical file - since SQLite is just a file based DB.
	/// If you ever do a write and then need to read it shortly after, like .5 to 1 second after... there's a chance it hasn't been
	/// updated yet... and this is local.  So, just make sure you use a coroutine or whatever to make sure data is written before
	/// using it.
	/// 
	/// SQLite is nice for small games, high scores, simple saved, etc.  It is not very secure and not very fast, but it's cheap,
	/// simple, and useful at times.
	/// 
	/// Here are some starting tools and information.  Go explore.
	/// </summary>
	public class SQLite : MonoBehaviour
	{
		public static SQLite Instance = null;
		public bool DebugMode = false;

		// Location of database - this will be set during Awake as to stop Unity 5.4 error regarding initialization before scene is set
		// file should show up in the Unity inspector after a few seconds of running it the first time
		private static string _sqlDBLocation = "";

		/// <summary>
		/// Table name and DB actual file name -- this is the name of the actual file on the filesystem
		/// </summary>
		private const string SQL_DB_NAME = "PlayersSQLite";

		// table name
		private const string SQL_TABLE_NAME = "Player";
		private const string FRIENDS_TABLE_NAME = "Friends";
        private const string MESSAGES_TABLE_NAME = "Messages";

		/// <summary>
		/// predefine columns here to there are no typos
		/// </summary>
		private const string COL_NAME = "name"; 
		private const string COL_PASSWORD = "password";
		private const string COL_HAT = "hat";
		private const string COL_TOP = "top";
		private const string COL_TOP_MATERIAL = "top_material";
		private const string COL_BOTTOM = "bottom";
		private const string COL_GLASSES = "glasses";
		private const string COL_SKIN = "skin";
		private const string COL_HAIR_MATERIAL = "hair_material";
		private const string COL_EYE_MATERIAL = "eye_material";
		private const string COL_BROW_MATERIAL = "brow_material";
		private const string COL_PLAYER_ID = "player_id"; 
        private const string COL_FRIEND_ID = "friend_id";
        private const string COL_MESSAGE_ID = "message_id";
        private const string COL_MESSAGE_TEXT = "message_text";
        private const string COL_SENDER_ID = "sender_id";
        private const string COL_TIMESTAMP = "timestamp";

		/// <summary>
		/// DB objects
		/// </summary>
		private IDbConnection _connection = null;
		private IDbCommand _command = null;
		private IDataReader _reader = null;
		private string _sqlString;

		private bool _createNewTavle = false;

		/// <summary>
		/// Awake will initialize the connection.  
		/// RunAsyncInit is just for show.  You can do the normal SQLiteInit to ensure that it is
		/// initialized during the Awake() phase and everything is ready during the Start() phase
		/// </summary>
		void Awake()
		{
			if (DebugMode)
				Debug.Log("--- Awake ---");

			// here is where we set the file location
			// ------------ IMPORTANT ---------
			// - during builds, this is located in the project root - same level as Assets/Library/obj/ProjectSettings
			// - during runtime (Windows at least), this is located in the SAME directory as the executable
			// you can play around with the path if you like, but build-vs-run locations need to be taken into account
			_sqlDBLocation = "URI=file:" + Application.dataPath + "/Plugins/SQLiter/Databases/" + SQL_DB_NAME + ".db";

			Debug.Log(_sqlDBLocation);
			Instance = this;
			SQLiteInit();
		}

		void Start()
		{
		}

		/// <summary>
		/// Uncomment if you want to see the time it takes to do things
		/// </summary>
		//void Update()
		//{
		//    Debug.Log(Time.time);
		//}

		/// <summary>
		/// Clean up SQLite Connections, anything else
		/// </summary>
		void OnDestroy()
		{
			SQLiteClose();
		}

		/// <summary>
		/// Basic initialization of SQLite
		/// </summary>
		private void SQLiteInit()
		{
			Debug.Log("SQLiter - Opening SQLite Connection");
			_connection = new SqliteConnection(_sqlDBLocation);
			_command = _connection.CreateCommand();
			_connection.Open();

			// WAL = write ahead logging, very huge speed increase
			_command.CommandText = "PRAGMA journal_mode = WAL;";
			_command.ExecuteNonQuery();

			// journal mode = look it up on google, I don't remember
			_command.CommandText = "PRAGMA journal_mode";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
			_reader.Close();

			// more speed increases
			_command.CommandText = "PRAGMA synchronous = OFF";
			_command.ExecuteNonQuery();

			// and some more
			_command.CommandText = "PRAGMA synchronous";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
			_reader.Close();

			// here we check if the table you want to use exists or not.  If it doesn't exist we create it.
			 _command.CommandText = $"CREATE TABLE IF NOT EXISTS {SQL_TABLE_NAME} (" +
				$"{COL_PLAYER_ID} INTEGER PRIMARY KEY AUTOINCREMENT, " +
				$"{COL_NAME} TEXT UNIQUE, " +
				$"{COL_PASSWORD} TEXT, " +
				$"{COL_HAT} INTEGER, " +
				$"{COL_TOP} INTEGER, " +
				$"{COL_TOP_MATERIAL} INTEGER, " +
				$"{COL_BOTTOM} INTEGER, " +
				$"{COL_GLASSES} INTEGER, " +
				$"{COL_SKIN} INTEGER, " +
				$"{COL_HAIR_MATERIAL} INTEGER, " +
				$"{COL_EYE_MATERIAL} INTEGER, " +
				$"{COL_BROW_MATERIAL} INTEGER)";
			_command.ExecuteNonQuery();

            // FRIENDS_TABLE
            _command.CommandText = $"CREATE TABLE IF NOT EXISTS {FRIENDS_TABLE_NAME} (" +
                $"{COL_PLAYER_ID} INTEGER, " +
                $"{COL_FRIEND_ID} INTEGER, " +
                $"FOREIGN KEY({COL_PLAYER_ID}) REFERENCES {SQL_TABLE_NAME}({COL_PLAYER_ID}), " +
                $"FOREIGN KEY({COL_FRIEND_ID}) REFERENCES {SQL_TABLE_NAME}({COL_PLAYER_ID}))";
            _command.ExecuteNonQuery();

            // MESSAGES_TABLE
            _command.CommandText = $"CREATE TABLE IF NOT EXISTS {MESSAGES_TABLE_NAME} (" +
                $"{COL_MESSAGE_ID} INTEGER PRIMARY KEY AUTOINCREMENT, " +
                $"{COL_PLAYER_ID} INTEGER, " +
                $"{COL_SENDER_ID} INTEGER, " +
                $"{COL_MESSAGE_TEXT} TEXT, " +
                $"{COL_TIMESTAMP} TEXT, " +
                $"FOREIGN KEY({COL_PLAYER_ID}) REFERENCES {SQL_TABLE_NAME}({COL_PLAYER_ID}), " +
                $"FOREIGN KEY({COL_SENDER_ID}) REFERENCES {SQL_TABLE_NAME}({COL_PLAYER_ID}))";
            _command.ExecuteNonQuery();


			// close connection
			_connection.Close();
		}

		#region Insert
		/// <summary>
		/// Inserts a player into the database
		/// http://www.sqlite.org/lang_insert.html
		/// name must be unique, it's our primary key
		/// </summary>
		private int player_count = 0;
		public void InsertPlayer(string name, string password)
		{
			name = name.ToLower();
			player_count += 1;
		    int player_id = player_count;
			// note - this will replace any item that already exists, overwriting them.  
			// normal INSERT without the REPLACE will throw an error if an item already exists
			_sqlString = "INSERT OR REPLACE INTO " + SQL_TABLE_NAME
				+ " ("
				+ COL_PLAYER_ID + ","
				+ COL_NAME + ","
				+ COL_PASSWORD
				+ ") VALUES ("
				+ player_id + ",'"  
				+ name + "',"        
				+ "'" + password + "'"
				+ ");";

			if (DebugMode)
				Debug.Log(_sqlString);
			ExecuteNonQuery(_sqlString);
		}
		public void InsertClothInfo(int playerId, Dictionary<string, int> clothData)
		{
			string _sqlString = "INSERT OR REPLACE INTO " + SQL_TABLE_NAME + " (";

			List<string> columns = new List<string>
			{
				COL_HAT, COL_TOP, COL_TOP_MATERIAL, COL_BOTTOM, COL_GLASSES,
				COL_SKIN, COL_HAIR_MATERIAL, COL_EYE_MATERIAL, COL_BROW_MATERIAL
			};

			_sqlString += string.Join(", ", columns);
			_sqlString += ") VALUES (";

			List<string> values = new List<string>();
			foreach (string column in columns)
			{
				if (clothData.ContainsKey(column))
				{
					values.Add(clothData[column].ToString());
				}
				else
				{
					values.Add("NULL"); 
				}
			}

			_sqlString = _sqlString.Insert(_sqlString.IndexOf('(') + 1, playerId.ToString() + ", ");
			_sqlString += string.Join(", ", values) + ");";

			if (DebugMode)
				Debug.Log(_sqlString);

			ExecuteNonQuery(_sqlString);
		}
		#endregion

		#region Query Values

		/// <summary>
		/// Quick method to show how you can query everything.  Expland on the query parameters to limit what you're looking for, etc.
		/// </summary>
		public void GetAllPlayers()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			_connection.Open();

			// if you have a bunch of stuff, this is going to be inefficient and a pain.  it's just for testing/show
			_command.CommandText = "SELECT * FROM " + SQL_TABLE_NAME;
			_reader = _command.ExecuteReader();
			while (_reader.Read())
			{
				// reuse same stringbuilder
				sb.Length = 0;
				sb.Append(_reader.GetInt32(0)).Append(" ");
				sb.Append(_reader.GetString(1)).Append(" ");
				sb.Append(_reader.GetString(2)).Append(" ");
				sb.AppendLine();

				// view our output
				if (DebugMode)
					Debug.Log(sb.ToString());
			}
			_reader.Close();
			_connection.Close();
		}

		public int ValidateUser(string name, string password)
		{
			int playerId = -1;
			try
			{
				_connection.Open();
				Debug.Log("Connection opened successfully!");
				// Use parameterized query to prevent SQL injection
				_command.CommandText = $"SELECT {COL_PLAYER_ID} FROM {SQL_TABLE_NAME} WHERE {COL_NAME} = name AND {COL_PASSWORD} = password";

				_reader = _command.ExecuteReader();
				if (_reader.Read())
				{
					playerId = Convert.ToInt32(_reader[COL_PLAYER_ID]);
				}

				_reader.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Error validating user: " + ex.Message);
			}
			finally
			{
				_connection.Close();
			}

			return playerId;
		}

		public Dictionary<string, int> GetPlayerCustomization(string name, string password)
		{
			Dictionary<string, int> playerData = new Dictionary<string, int>();
			try
			{
				_connection.Open();

				_command.CommandText = $"SELECT {COL_HAT}, {COL_TOP}, {COL_TOP_MATERIAL}, {COL_BOTTOM}, {COL_GLASSES}, " +
                       $"{COL_SKIN}, {COL_HAIR_MATERIAL}, {COL_EYE_MATERIAL}, {COL_BROW_MATERIAL} " +
                       $"FROM {SQL_TABLE_NAME} WHERE {COL_NAME} = name AND {COL_PASSWORD} = password";

				
				using (var reader = _command.ExecuteReader())
				{
					if (reader.Read())
					{
						playerData[COL_HAT] = reader.GetInt32(0);
						playerData[COL_TOP] = reader.GetInt32(1);
						playerData[COL_TOP_MATERIAL] = reader.GetInt32(2);
						playerData[COL_BOTTOM] = reader.GetInt32(3);
						playerData[COL_GLASSES] = reader.GetInt32(4);
						playerData[COL_SKIN] = reader.GetInt32(5);
						playerData[COL_HAIR_MATERIAL] = reader.GetInt32(6);
						playerData[COL_EYE_MATERIAL] = reader.GetInt32(7);
						playerData[COL_BROW_MATERIAL] = reader.GetInt32(8);
					}
				}

				_reader.Close();
			}
			catch (Exception ex)
			{
				Debug.LogError("Error validating user: " + ex.Message);
			}
			finally
			{
				_connection.Close();
			}
			return playerData;
		}

		/// <summary>
		/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public int GetPlayerId(string column, string value)
		{
			int sel = -1;
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				sel = _reader.GetInt32(0);
			else
				Debug.Log("QueryInt - nothing to read...");
			_reader.Close();
			_connection.Close();
			return sel;
		}

		/// <summary>
		/// Supply the column and the value you're trying to find, and it will use the primary key to query the result
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public short QueryShort(string column, string value)
		{
			short sel = -1;
			_connection.Open();
			_command.CommandText = "SELECT " + column + " FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + value + "'";
			_reader = _command.ExecuteReader();
			if (_reader.Read())
				sel = _reader.GetInt16(0);
			else
				Debug.Log("QueryShort - nothing to read...");
			_reader.Close();
			_connection.Close();
			return sel;
		}
		#endregion

		#region Update / Replace Values
		/// <summary>
		/// A 'Set' method that will set a column value for a specific player, using their name as the unique primary key
		/// to some value.  This currently just uses 'int' types, but you could modify this to use/do most anything.
		/// Remember strings need single/double quotes around their values
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(string column, int value, string name)
		{
			ExecuteNonQuery("UPDATE OR REPLACE " + SQL_TABLE_NAME + " SET " + column + "=" + value + " WHERE " + COL_NAME + "='" + name + "'");
		}

		#endregion

		#region Delete

		/// <summary>
		/// Basic delete, using the name primary key for the 
		/// </summary>
		/// <param name="nameKey"></param>
		public void DeletePlayer(string nameKey)
		{
			ExecuteNonQuery("DELETE FROM " + SQL_TABLE_NAME + " WHERE " + COL_NAME + "='" + nameKey + "'");
		}
		public void DropTable(string tableName)
		{
			string dropTableSQL = $"DROP TABLE IF EXISTS {tableName};";
			
			if (_connection == null)
			{
				Debug.LogError("Connection is not initialized.");
				return;
			}
			
			try
			{
				_command = _connection.CreateCommand();
				_command.CommandText = dropTableSQL;
				_command.ExecuteNonQuery();
				Debug.Log($"Table {tableName} has been dropped successfully.");
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error dropping table {tableName}: {ex.Message}");
			}
			finally
			{
				_connection.Close();
				_connection = null;
			}
		}
		#endregion

		/// <summary>
		/// Basic execute command - open, create command, execute, close
		/// </summary>
		/// <param name="commandText"></param>
		public void ExecuteNonQuery(string commandText)
		{
			_connection.Open();
			_command.CommandText = commandText;
			_command.ExecuteNonQuery();
			_connection.Close();
		}

		#region Friends
        public void AddFriend(int playerId, int friendId)
        {
            _connection.Open();
            _command.CommandText = $"INSERT INTO {FRIENDS_TABLE_NAME} ({COL_PLAYER_ID}, {COL_FRIEND_ID}) " +
                $"VALUES ({playerId}, {friendId})";
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public void GetFriends(int playerId)
        {
            _connection.Open();
            _command.CommandText = $"SELECT {COL_FRIEND_ID} FROM {FRIENDS_TABLE_NAME} WHERE {COL_PLAYER_ID} = {playerId}";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                Debug.Log("Friend ID: " + _reader.GetInt32(0));
            }
            _reader.Close();
            _connection.Close();
        }
        #endregion

        #region Messages
        public void SendMessage(int playerId, int senderId, string messageText)
        {
            _connection.Open();
            string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            _command.CommandText = $"INSERT INTO {MESSAGES_TABLE_NAME} " +
                $"({COL_PLAYER_ID}, {COL_SENDER_ID}, {COL_MESSAGE_TEXT}, {COL_TIMESTAMP}) " +
                $"VALUES ({playerId}, {senderId}, '{messageText}', '{timestamp}')";
            _command.ExecuteNonQuery();
            _connection.Close();
        }

        public void GetMessages(int playerId)
        {
            _connection.Open();
            _command.CommandText = $"SELECT {COL_SENDER_ID}, {COL_MESSAGE_TEXT}, {COL_TIMESTAMP} " +
                $"FROM {MESSAGES_TABLE_NAME} WHERE {COL_PLAYER_ID} = {playerId} ORDER BY {COL_TIMESTAMP} DESC";
            _reader = _command.ExecuteReader();
            while (_reader.Read())
            {
                Debug.Log($"From: { _reader.GetInt32(0) }, Message: { _reader.GetString(1) }, Time: { _reader.GetString(2) }");
            }
            _reader.Close();
            _connection.Close();
        }
        #endregion

		/// <summary>
		/// Clean up everything for SQLite
		/// </summary>
		private void SQLiteClose()
		{
			if (_reader != null && !_reader.IsClosed)
				_reader.Close();
			_reader = null;

			if (_command != null)
				_command.Dispose();
			_command = null;

			if (_connection != null && _connection.State != ConnectionState.Closed)
				_connection.Close();
			_connection = null;
		}
	}
}
