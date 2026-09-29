using Com.IsartDigital.ProjectName;
using Godot;
using Godot.Collections;
using System.Collections.Generic;

namespace Com.IsartDigital.Kinisi.Login
{
    public partial class LeaderBoard : Control
    {
        #region Singleton
        static private LeaderBoard instance;

        private LeaderBoard() { }

        static public LeaderBoard GetInstance()
        {
            if (instance == null) instance = new LeaderBoard();
            return instance;
        }

        #endregion

        [Export] private Button quit;
        [Export] private VBoxContainer playerList;
        [Export] private PackedScene sceneLBEntry;
        [Export] private Panel loadingPopup;

        private const string VALUE_API = "dev_42cd25e3689d473fac412eb37a911681";
        private const string LEADERBOARD_REF = "Kinisi18453";
        private const string PATH_LOOTLOCKER_DATA = "user://LootLocker.data";
        private const string HEADER_JSON = "Content-Type: application/json";
        private const string HEADER_TOKEN = "x-session-token:";
        private const string KEY_GAME = "game_key";
        private const string KEY_PLAYER_ID = "player_identifier";
        private const string KEY_DEV_MODE = "development_mode";
        private const string KEY_GAME_VERSION = "game_version";
        private const string VALUE_GAME_VERSION = "1.0";

        private string tokenSession;
        private int existingScore = -1;
        private bool leaderboardLoaded;

        private HttpRequest httpAuth;
        private HttpRequest httpScore;
        private HttpRequest httpLeaderboard;
        private HttpRequest httpSetUsername;
        private HttpRequest httpGetUsername;
        private HttpRequest httpCheckScore;

        public override void _Ready()
        {
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(LeaderBoard) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            httpAuth = new();
            AddChild(httpAuth);
            httpAuth.RequestCompleted += HttpAuthRequestCompleted;
            quit.ButtonDown += QuitButtonDown;
            AutentificationRequest();
        }

        private void QuitButtonDown()
        {
            Menu.Menu.GetInstance().buttonsMenu.Visible = true;
            QueueFree();
        }

        private void AutentificationRequest()
        {
            if (!string.IsNullOrEmpty(tokenSession))
            {
                SetPlayerNameAndUpload();
                return;
            }

            Godot.Collections.Dictionary<string, Variant> data = new() //Has stated in the Lootlocker doc
            {
                {KEY_GAME, VALUE_API},
                {KEY_GAME_VERSION, VALUE_GAME_VERSION},
                {KEY_DEV_MODE, true}
            };

            string[] headers = new string[] { HEADER_JSON }; //Comes from the doc of Lootlocker
            string playerID = "";

            var file = FileAccess.Open(PATH_LOOTLOCKER_DATA, FileAccess.ModeFlags.Read);
            if (file != null)
                playerID = file.GetAsText();

            if (!string.IsNullOrEmpty(playerID))
            {
                data[KEY_PLAYER_ID] = playerID;
            }

            //Below : Create and send a http request
            httpAuth.Request("https://api.lootlocker.io/game/v2/session/guest", headers, HttpClient.Method.Post, Json.Stringify(data));
        }

        private void HttpAuthRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            var json = (Dictionary)Json.ParseString(pBody.GetStringFromUtf8());
            if (json == null || !json.ContainsKey("session_token") || !json.ContainsKey(KEY_PLAYER_ID))
            {
                GD.PrintErr("Invalid auth response from LootLocker");
                return;
            }

            tokenSession = json["session_token"].ToString();//add the sesion token to memory
            var file = FileAccess.Open(PATH_LOOTLOCKER_DATA, FileAccess.ModeFlags.Write);
            file.StoreString(json[KEY_PLAYER_ID].ToString());
            file.Flush();
            file.Close();


            CheckExistingScoreBeforeUpload();
            httpAuth.QueueFree();
        }

        private void CheckExistingScoreBeforeUpload()
        {
            httpCheckScore = new();
            AddChild(httpCheckScore);
            httpCheckScore.RequestCompleted += HttpCheckScoreRequestCompleted;
                

            string url = $"https://api.lootlocker.io/game/leaderboards/{LEADERBOARD_REF}/personal";
            string[] headersScore = { HEADER_JSON, HEADER_TOKEN + tokenSession };
            httpCheckScore.Request(url, headersScore, HttpClient.Method.Get);
        }

        private void HttpCheckScoreRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            httpCheckScore.QueueFree();

            var json = (Dictionary)Json.ParseString(pBody.GetStringFromUtf8());
            existingScore = -1;
            if (json != null && json.ContainsKey("items"))
            {
                var items = (Array)json["items"];
                if (items.Count > 0)
                {
                    var entry = (Dictionary)items[0];
                    if (entry.ContainsKey("score"))
                        existingScore = int.Parse(entry["score"].ToString());
                }
            }

            if (existingScore != -1)
            {
                GameManager.score = existingScore;
            }
            else if (GameManager.score > existingScore) SetPlayerNameAndUpload();
            else
            {
                GetLeaderboard();
            }
        }
        

        private void SetPlayerNameAndUpload()
        {
            httpGetUsername = new();
            AddChild(httpGetUsername);
            httpGetUsername.RequestCompleted += HttpGetUsernameRequestCompleted;
                
            string url = "https://api.lootlocker.io/game/player/name";
            string[] headersGet = { HEADER_JSON, HEADER_TOKEN + tokenSession };
            httpGetUsername.Request(url, headersGet, HttpClient.Method.Get);
        }

        private void HttpGetUsernameRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            httpGetUsername.QueueFree();

            var json = (Dictionary)Json.ParseString(pBody.GetStringFromUtf8());
            bool nameSet = json != null && json.ContainsKey("name") && json["name"].AsString() == Login.savedUsername;

            if (!nameSet)
            {
                httpSetUsername = new();
                AddChild(httpSetUsername);
                var nameData = new Godot.Collections.Dictionary<string, Variant> { { "name", Login.savedUsername } };
                string[] headersSet = { HEADER_JSON, HEADER_TOKEN + tokenSession };
                httpSetUsername.RequestCompleted += HttpSetUsernameRequestCompleted;
                httpSetUsername.Request("https://api.lootlocker.io/game/player/name", headersSet, HttpClient.Method.Patch, Json.Stringify(nameData));
            }
            else
            {
                UploadScore(GameManager.score);
            }
        }

        private void HttpSetUsernameRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            httpSetUsername.QueueFree();
            UploadScore(GameManager.score);
        }

        private void GetLeaderboard()
        {
            foreach (Node lNode in playerList.GetChildren()) QueueFree();

            string url = $"https://api.lootlocker.io/game/leaderboards/{LEADERBOARD_REF}/list?count=10";
            string[] headers = { HEADER_JSON, HEADER_TOKEN + tokenSession };

            httpLeaderboard = new();
            AddChild(httpLeaderboard);
            httpLeaderboard.RequestCompleted += HttpLeaderboardRequestCompleted;
            httpLeaderboard.Request(url, headers, HttpClient.Method.Get, "");
        }

        private void HttpLeaderboardRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            var json = (Dictionary)Json.ParseString(pBody.GetStringFromUtf8());
            if (json == null || !json.ContainsKey("items")) return;

            //Below : Check in the Lootlocker leaderboard file the number of entry, and for each of them
            //replicate the entry in the Godot leaderboard scene (using the right key in string to assing the right value).
            var items = (Array)json["items"];
            foreach (Dictionary entry in items)
            {
                var player = (Dictionary)entry["player"];
                string name = player.GetValueOrDefault("name", "Unknown").ToString();
                string score = entry.GetValueOrDefault("score", "0").ToString();

                var entryNode = sceneLBEntry.Instantiate<LeaderboardEntry>();
                entryNode.playerName = name;
                entryNode.playerScore = score;
                playerList.AddChild(entryNode);
            }

            loadingPopup.Hide();
        }

        private void UploadScore(int pScore)
        {
            var data = new Godot.Collections.Dictionary<string, Variant> { { "score", pScore } };
            string[] headers = { HEADER_JSON, HEADER_TOKEN + tokenSession };

            httpScore = new();
            AddChild(httpScore);
            httpScore.RequestCompleted += HttpScoreRequestCompleted;
            httpScore.Request($"https://api.lootlocker.io/game/leaderboards/{LEADERBOARD_REF}/submit", headers, HttpClient.Method.Post, Json.Stringify(data));
        }

        private void HttpScoreRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            httpScore.QueueFree();
            GetLeaderboard();
        }

        private void SetPlayerName()
        {
            var data = new Godot.Collections.Dictionary<string, Variant> { { "name", Login.savedUsername } };
            string url = "https://api.lootlocker.io/game/player/name";
            string[] headers = { HEADER_JSON, HEADER_TOKEN + tokenSession };

            httpSetUsername = new();
            AddChild(httpSetUsername);
            httpSetUsername.RequestCompleted += httpSetUsernameRequestCompleted;
            httpSetUsername.Request(url, headers, HttpClient.Method.Patch, Json.Stringify(data));
        }

        private void httpSetUsernameRequestCompleted(long pResult, long pResponseCode, string[] pHeaders, byte[] pBody)
        {
            httpSetUsername.QueueFree();
        }

        protected override void Dispose(bool pDisposing)
        {
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
