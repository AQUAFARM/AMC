using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

// From: http://weblogs.asp.net/hernandl/archive/2005/11/21/usercredentialsdialog.aspx
namespace JelleDruyts.Windows.Controls
{
    #region UserCredentialsDialogFlags
    /// <summary>
    /// Specifies special behavior for this function. 
    /// This value can be a bitwise-OR combination of zero or more of the following values. 
    /// </summary>
    // For more information of these flags see:
    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/secauthn/security/creduipromptforcredentials.asp
    // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnnetsec/html/dpapiusercredentials.asp?frame=true
    [Flags]
    public enum UserCredentialsDialogFlags
    {
        /// <summary>
        /// 
        /// </summary>
        Default = GenericCredentials |
                    ShowSaveCheckbox |
                    AlwaysShowUI |
                    ExpectConfirmation,
        /// <summary>
        /// 
        /// </summary>
        None = 0x0,
        /// <summary>
        /// 
        /// </summary>
        IncorrectPassword = 0x1,
        /// <summary>
        /// 
        /// </summary>
        DoNotPersist = 0x2,
        /// <summary>
        /// 
        /// </summary>
        RequestAdministrator = 0x4,
        /// <summary>
        /// 
        /// </summary>
        ExcludesCertificates = 0x8,
        /// <summary>
        /// 
        /// </summary>
        RequireCertificate = 0x10,
        /// <summary>
        /// 
        /// </summary>
        ShowSaveCheckbox = 0x40,
        /// <summary>
        /// 
        /// </summary>
        AlwaysShowUI = 0x80,
        /// <summary>
        /// 
        /// </summary>
        RequireSmartCard = 0x100,
        /// <summary>
        /// 
        /// </summary>
        PasswordOnlyOk = 0x200,
        /// <summary>
        /// 
        /// </summary>
        ValidateUsername = 0x400,
        /// <summary>
        /// 
        /// </summary>
        CompleteUserName = 0x800,
        /// <summary>
        /// 
        /// </summary>
        Persist = 0x1000,
        /// <summary>
        /// 
        /// </summary>
        ServerCredential = 0x4000,
        /// <summary>
        /// 
        /// </summary>
        ExpectConfirmation = 0x20000,
        /// <summary>
        /// 
        /// </summary>
        GenericCredentials = 0x40000,
        /// <summary>
        /// 
        /// </summary>
        UsernameTargetCredentials = 0x80000,
        /// <summary>
        /// 
        /// </summary>
        KeepUsername = 0x100000
    }
    #endregion

    #region UserCredentialsDialog class

    /// <summary>
    /// Displays a dialog box and promts the user for login credentials.
    /// </summary>
    [ToolboxItem(true)]
    [DesignerCategory("Dialogs")]
    public class UserCredentialsDialog : CommonDialog
    {
        #region Fields

        private string user;
        private SecureString password;
        private string domain;
        private string target;
        private string message;
        private string caption;
        private Image banner;
        private bool saveChecked;
        private UserCredentialsDialogFlags flags;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentialsDialog"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserCredentialsDialog()
        {
            this.Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentialsDialog"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public UserCredentialsDialog(string target)
            : this(target, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentialsDialog"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caption">The caption.</param>
        public UserCredentialsDialog(string target, string caption)
            : this(target, caption, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentialsDialog"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="message">The message.</param>
        public UserCredentialsDialog(string target, string caption, string message)
            : this(target, caption, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCredentialsDialog"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="message">The message.</param>
        /// <param name="banner">The banner.</param>
        public UserCredentialsDialog(string target, string caption, string message, Image banner)
            : this()
        {
            this.Target = target;
            this.Caption = caption;
            this.Message = message;
            this.Banner = banner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User
        {
            get { return user; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_USERNAME_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The user name has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_USERNAME_LENGTH), "User");
                    }
                }
                user = value;
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public SecureString Password
        {
            get { return password; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_PASSWORD_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The password has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_PASSWORD_LENGTH), "Password");
                    }
                }
                password = value;
            }
        }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain
        {
            get { return domain; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_DOMAIN_TARGET_LENGTH)
                    {
                        throw new ArgumentException(string.Format(
                            "The domain name has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_DOMAIN_TARGET_LENGTH), "Domain");
                    }
                }
                domain = value;
            }
        }

        /// <summary>
        /// Gets or sets the target resource to connect to.
        /// </summary>
        public string Target
        {
            get { return target; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_GENERIC_TARGET_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The target has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_GENERIC_TARGET_LENGTH), "Target");
                    }
                }
                target = value;
            }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_MESSAGE_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The message has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_MESSAGE_LENGTH), "Message");
                    }
                }
                message = value;
            }
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set
            {
                if (value != null)
                {
                    if (value.Length > NativeMethods.CREDUI_MAX_CAPTION_LENGTH)
                    {
                        throw new ArgumentException(
                            string.Format("The caption has a maximum length of {0} characters.",
                            NativeMethods.CREDUI_MAX_CAPTION_LENGTH), "Caption");
                    }
                }
                caption = value;
            }
        }

        /// <summary>
        /// Gets or sets the banner.
        /// </summary>
        public Image Banner
        {
            get { return banner; }
            set
            {
                if (value != null)
                {
                    if (value.Width != NativeMethods.CREDUI_BANNER_WIDTH)
                    {
                        throw new ArgumentException(
                            string.Format("The banner image width must be {0} pixels.",
                            NativeMethods.CREDUI_BANNER_WIDTH), "Banner");
                    }
                    if (value.Height != NativeMethods.CREDUI_BANNER_HEIGHT)
                    {
                        throw new ArgumentException(
                            string.Format("The banner image height must be {0} pixels.",
                            NativeMethods.CREDUI_BANNER_HEIGHT), "Banner");
                    }
                }
                banner = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the save credentials check box is checked.
        /// </summary>
        public bool SaveChecked
        {
            get { return saveChecked; }
            set { saveChecked = value; }
        }

        /// <summary>
        /// Gets or sets the flags to use.
        /// </summary>
        public UserCredentialsDialogFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Confirms the credentials.
        /// </summary>
        /// <param name="confirm">Determines if the credentials must be confirmed.</param>
        public void ConfirmCredentials(bool confirm)
        {
            new UIPermission(UIPermissionWindow.SafeSubWindows).Demand();

            NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIConfirmCredentialsW(this.target, confirm);

            if (result != NativeMethods.CredUIReturnCodes.NO_ERROR &&
                result != NativeMethods.CredUIReturnCodes.ERROR_NOT_FOUND &&
                result != NativeMethods.CredUIReturnCodes.ERROR_INVALID_PARAMETER)
            {
                throw new InvalidOperationException(TranslateReturnCode(result));
            }
        }

        /// <summary>
        /// This method is for backward compatibility with APIs that does
        /// not provide the <see cref="SecureString"/> type.
        /// </summary>
        /// <returns>The password as a regular string.</returns>
        public string PasswordToString()
        {
            IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(this.password);
            try
            {
                // Unsecure managed string
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        #endregion

        #region CommonDialog overrides

        /// <summary>
        /// When overridden in a derived class, specifies a common dialog box.
        /// </summary>
        /// <param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box.</param>
        /// <returns>
        /// true if the dialog box was successfully run; otherwise, false.
        /// </returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            if (Environment.OSVersion.Version.Major < 5)
            {
                throw new PlatformNotSupportedException("The Credential Management API requires Windows XP / Windows Server 2003 or later.");
            }

            NativeMethods.CredUIInfo credInfo = new NativeMethods.CredUIInfo(hwndOwner,
                this.caption, this.message, this.banner);
            StringBuilder usr = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
            StringBuilder pwd = new StringBuilder(NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);

            if (!string.IsNullOrEmpty(this.User))
            {
                if (!string.IsNullOrEmpty(this.Domain))
                {
                    usr.Append(this.Domain + "\\");
                }
                usr.Append(this.User);
            }
            if (this.Password != null)
            {
                pwd.Append(this.PasswordToString());
            }

            try
            {
                NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIPromptForCredentials(
                                                        ref credInfo, this.target,
                                                        IntPtr.Zero, 0,
                                                        usr, NativeMethods.CREDUI_MAX_USERNAME_LENGTH,
                                                        pwd, NativeMethods.CREDUI_MAX_PASSWORD_LENGTH,
                                                        ref this.saveChecked, this.flags);
                switch (result)
                {
                    case NativeMethods.CredUIReturnCodes.NO_ERROR:
                        LoadUserDomainValues(usr);
                        LoadPasswordValue(pwd);
                        return true;
                    case NativeMethods.CredUIReturnCodes.ERROR_CANCELLED:
                        this.User = null;
                        this.Password = null;
                        return false;
                    default:
                        throw new InvalidOperationException(TranslateReturnCode(result));
                }
            }
            finally
            {
                usr.Remove(0, usr.Length);
                pwd.Remove(0, pwd.Length);
                if (this.banner != null)
                {
                    NativeMethods.DeleteObject(credInfo.hbmBanner);
                }
            }
        }

        /// <summary>
        /// Set all properties to it's default values.
        /// </summary>
        public override void Reset()
        {
            this.target = Application.ProductName ?? AppDomain.CurrentDomain.FriendlyName;
            this.user = null;
            this.password = null;
            this.domain = null;
            this.caption = null;// target as caption;
            this.message = null;
            this.banner = null;
            this.saveChecked = false;
            this.flags = UserCredentialsDialogFlags.Default;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.password != null)
            {
                this.password.Dispose();
                this.password = null;
            }
        }

        #endregion

        #region Private methods

        private static string TranslateReturnCode(NativeMethods.CredUIReturnCodes result)
        {
            return string.Format("Invalid operation: {0}", result.ToString());
        }

        private void LoadPasswordValue(StringBuilder password)
        {
            char[] pwd = new char[password.Length];
            using (SecureString securePassword = new SecureString())
            {
                try
                {
                    password.CopyTo(0, pwd, 0, pwd.Length);
                    foreach (char c in pwd)
                    {
                        securePassword.AppendChar(c);
                    }
                    securePassword.MakeReadOnly();
                    this.Password = securePassword.Copy();
                }
                finally
                {
                    // discard the char array
                    Array.Clear(pwd, 0, pwd.Length);
                }
            }
        }

        private void LoadUserDomainValues(StringBuilder principalName)
        {
            StringBuilder user = new StringBuilder(NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
            StringBuilder domain = new StringBuilder(NativeMethods.CREDUI_MAX_DOMAIN_TARGET_LENGTH);
            NativeMethods.CredUIReturnCodes result = NativeMethods.CredUIParseUserNameW(principalName.ToString(),
                user, NativeMethods.CREDUI_MAX_USERNAME_LENGTH, domain, NativeMethods.CREDUI_MAX_DOMAIN_TARGET_LENGTH);

            if (result == NativeMethods.CredUIReturnCodes.NO_ERROR)
            {
                this.User = user.ToString();
                this.Domain = domain.ToString();
            }
            else
            {
                this.User = principalName.ToString();
                this.Domain = Environment.MachineName;
            }
        }

        #endregion

        #region Unmanaged code

        [SuppressUnmanagedCodeSecurity]
        private sealed class NativeMethods
        {
            internal const int CREDUI_MAX_MESSAGE_LENGTH = 100;
            internal const int CREDUI_MAX_CAPTION_LENGTH = 100;
            internal const int CREDUI_MAX_GENERIC_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_DOMAIN_TARGET_LENGTH = 100;
            internal const int CREDUI_MAX_USERNAME_LENGTH = 100;
            internal const int CREDUI_MAX_PASSWORD_LENGTH = 100;
            internal const int CREDUI_BANNER_HEIGHT = 60;
            internal const int CREDUI_BANNER_WIDTH = 320;

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            internal static extern bool DeleteObject(IntPtr hObject);

            [DllImport("credui.dll", EntryPoint = "CredUIPromptForCredentialsW", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIPromptForCredentials(
                ref CredUIInfo creditUR,
                string targetName,
                IntPtr reserved1,
                int iError,
                StringBuilder userName,
                int maxUserName,
                StringBuilder password,
                int maxPassword,
                ref bool iSave,
                UserCredentialsDialogFlags flags);

            [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIParseUserNameW(
                string userName,
                StringBuilder user,
                int userMaxChars,
                StringBuilder domain,
                int domainMaxChars);

            [DllImport("credui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal extern static CredUIReturnCodes CredUIConfirmCredentialsW(string targetName, bool confirm);

            internal enum CredUIReturnCodes
            {
                NO_ERROR = 0,
                ERROR_CANCELLED = 1223,
                ERROR_NO_SUCH_LOGON_SESSION = 1312,
                ERROR_NOT_FOUND = 1168,
                ERROR_INVALID_ACCOUNT_NAME = 1315,
                ERROR_INSUFFICIENT_BUFFER = 122,
                ERROR_INVALID_PARAMETER = 87,
                ERROR_INVALID_FLAGS = 1004
            }

            internal struct CredUIInfo
            {
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
                internal CredUIInfo(IntPtr owner, string caption, string message, Image banner)
                {
                    this.cbSize = Marshal.SizeOf(typeof(CredUIInfo));
                    this.hwndParent = owner;
                    this.pszCaptionText = caption;
                    this.pszMessageText = message;

                    if (banner != null)
                    {
                        this.hbmBanner = new Bitmap(banner,
                            NativeMethods.CREDUI_BANNER_WIDTH, NativeMethods.CREDUI_BANNER_HEIGHT).GetHbitmap();
                    }
                    else
                    {
                        this.hbmBanner = IntPtr.Zero;
                    }
                }

                internal int cbSize;
                internal IntPtr hwndParent;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszMessageText;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string pszCaptionText;
                internal IntPtr hbmBanner;
            }
        }

        #endregion
    }
    #endregion
}