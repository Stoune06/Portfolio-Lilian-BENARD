using Com.IsartDigital.ProjectName;
using Godot;
using Godot.Collections;

// Author : Hector Rosier & Guillaume Julia

namespace Com.IsartDigital.Kinisi.Login
{
	public partial class NewUserAccount : CanvasLayer
    {
        [Export] private Button button;
        [Export] private LineEdit username;
        [Export] private LineEdit password;
        [Export] private ColorRect veil;
        [Export] private Label errorText;

        private Timer errorTimer;

        public const string ENCRYPTION_PASS = "9z4D8q6g";
        public const string SAVE_FILE = "user://UserData.save";
        private const string LEVELS_PATH = "levels";
        private const string PASSWORD_PATH = "password";
        private const string QUEUE_FREE_PATH = "queue_free";

        public override void _Ready()
		{
            button.ButtonDown += ButtonDown;

            errorTimer = new Timer();
            errorTimer.WaitTime = 2;
            errorTimer.Timeout += ErrorTimerTimeout;
            AddChild(errorTimer);
        }

        private void ButtonDown()
        {
            AudioManager.PlaySound("Click", true);
            Dictionary<string, Dictionary> userData = new();

            if (username.Text.Length < 1 || password.Text.Length < 1)
            {
                veil.Visible = true;

                Tween lTween = CreateTween();
                lTween.TweenProperty(errorText, TweenProp.FONT_SIZE, 100, 0.5f);
                lTween.TweenProperty(errorText, TweenProp.FONT_SIZE, 40, 0.5f);

                errorTimer.Start();
                return;
            }

            if (FileAccess.FileExists(SAVE_FILE))
            {
                //Below, this is for retrieve the already existing account and keep them save
                FileAccess lOldUserData = FileAccess.OpenEncryptedWithPass(SAVE_FILE, FileAccess.ModeFlags.Read, ENCRYPTION_PASS);
                userData = (Dictionary<string, Dictionary>)Json.ParseString(lOldUserData.GetAsText());
                lOldUserData.Close();
            }

            if (!userData.ContainsKey(username.Text))
            {
                //Create levels and if they are accesible
                int lLenghtLevels = DataManager.Levels.Count+1;   //Guillaume Julia
                Dictionary<string, bool> lLevels = new Dictionary<string, bool>();
                for (int i = 1; i <= lLenghtLevels; i++)
                {
                    lLevels[i.ToString()] = i == 1;
                }

                Dictionary lUserInfo = new Dictionary
                {
                    { PASSWORD_PATH, password.Text.Sha256Text() },
                    { LEVELS_PATH, lLevels }
                };

                userData[username.Text] = lUserInfo;
            }

            //Below, this will store the userData in a Json file in one line
            FileAccess lUserData = FileAccess.OpenEncryptedWithPass(SAVE_FILE, FileAccess.ModeFlags.Write, ENCRYPTION_PASS); //If the file doesn't exit it will be create here
            string lJson = Json.Stringify(userData);
            lUserData.StoreLine(lJson);
            lUserData.Close();

            GetParent<CanvasLayer>().Visible = true;
            CallDeferred(QUEUE_FREE_PATH);
        }

        private void ErrorTimerTimeout()
        {
            veil.Visible = false;
            errorTimer.Stop();
        }
    }
}
