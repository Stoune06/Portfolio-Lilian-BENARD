using Godot;
using Godot.Collections;
using System.Linq;

// Author : Hector Rosier

namespace Com.IsartDigital.Kinisi.Login
{
    public partial class Login : CanvasLayer
    {
        #region Singleton

        static private Login instance;

        private Login() { }

        static public Login GetInstance()
        {
            if (instance == null) instance = new Login();
            return instance;
        }

        #endregion

        [Export] private PackedScene sceneNewAccount;
		[Export] private PackedScene sceneMenu;
		[Export] private Button buttonNewAccount;
		[Export] private Button buttonConfirm;
		[Export] private LineEdit username;
		[Export] private LineEdit password;
        [Export] private Label errorText;
        [Export] private ColorRect veil;

        private const string LEVELS_PATH = "levels";
        private const string PASSWORD_PATH = "password";
        public static string savedUsername;

        private Timer errorTimer;

        public override void _Ready()
		{
            #region Singleton

            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Login) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            buttonNewAccount.ButtonDown += ButtonNewAccountDown;
            buttonConfirm.ButtonDown += ButtonConfirmDown;

            errorTimer = new Timer();
            errorTimer.WaitTime = 2;
            errorTimer.Timeout += ErrorTimerTimeout;
            AddChild(errorTimer);
        }

        private void ButtonNewAccountDown()
        {
            AudioManager.PlaySound("Click", true);
            CanvasLayer lNewUserAccount = sceneNewAccount.Instantiate() as CanvasLayer;
			AddChild(lNewUserAccount);
			Visible = false;
        }

        private void ButtonConfirmDown()
        {
            AudioManager.PlaySound("Click", true);
            if (FileAccess.FileExists(NewUserAccount.SAVE_FILE))
            {
                FileAccess lOldUserData = FileAccess.OpenEncryptedWithPass(NewUserAccount.SAVE_FILE, FileAccess.ModeFlags.Read, NewUserAccount.ENCRYPTION_PASS);

                //Below, this is to read the data in Json
                string lJsonString = lOldUserData.GetAsText();
                Json lJson = new();
                lJson.Parse(lJsonString);
                Dictionary<string, Dictionary> lData = lJson.Data.AsGodotDictionary<string, Dictionary>();

                //Below, this is to check if the username and password are in the data and match
                if (lData.TryGetValue(username.Text, out Dictionary userInfo))
                {
                    string lPasswordText = (string)userInfo[PASSWORD_PATH];
                    if (lPasswordText == password.Text.Sha256Text())  
                    {
                        savedUsername = username.Text;
                        Menu.Menu lMenu = sceneMenu.Instantiate() as Menu.Menu;
                        GetParent().AddChild(lMenu);
                        QueueFree();
                    }
                    else
                    {
                        ReturnError();
                    }
                }
                else
                {
                    ReturnError();
                }
            }
            else
            {
                ReturnError();
            }
        }



        private void ReturnError()
        {
            veil.Visible = true;

            Tween lTween = CreateTween();
            lTween.TweenProperty(errorText, TweenProp.FONT_SIZE, 100, 0.5f);
            lTween.TweenProperty(errorText, TweenProp.FONT_SIZE, 40, 0.5f);

            errorTimer.Start();
            return;
        }

        private void ErrorTimerTimeout()
        {
            veil.Visible = false;
            errorTimer.Stop();
        }

        protected override void Dispose(bool pDisposing)
        {
            AudioManager.PlaySound("Loading",false);
            instance = null;
            base.Dispose(pDisposing);
        }
    }
}
