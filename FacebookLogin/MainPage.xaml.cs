using Facebook;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FacebookLogin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //somente para teste não informe diretamente no app em produção
        private const string AppId = "your app id (facebook)";
        private const string ExtendedPermissions = "";//"publish_actions, user_managed_groups";

        public MainPage()
        {
            this.InitializeComponent();
            TextBlockSid.Text = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host;
        }

        private async void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            await AuthenticateFacebookAsync();
        }

        private async Task<string> AuthenticateFacebookAsync()
        {
            try
            {
                var fb = new FacebookClient();

                var redirectUri =
                    WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();//.GetCurrentApplicationCallbackUri().AbsoluteUri;//

                var loginUri = fb.GetLoginUrl(new
                {
                    client_id = AppId,
                    redirect_uri = redirectUri,
                    scope = ExtendedPermissions,
                    display = "popup",
                    response_type = "token"
                });

                var callbackUri = new Uri(redirectUri, UriKind.Absolute);

                var authenticationResult =
                  await
                    WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None,
                    loginUri, callbackUri);

                return ParseAuthenticationResult(fb, authenticationResult);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string ParseAuthenticationResult(FacebookClient fb,
                                            WebAuthenticationResult result)
        {
            Debug.Write(result.ResponseData);
            //Debug.Write(result.ResponseStatus);
            //?error=access_denied&error_code=200&error_description=Permissions+error&error_reason=user_denied#_=_
            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.ErrorHttp:
                    return "Error";
                case WebAuthenticationStatus.Success:

                    var oAuthResult = fb.ParseOAuthCallbackUrl(new Uri(result.ResponseData));
                    return oAuthResult.AccessToken;
                case WebAuthenticationStatus.UserCancel:
                    return "Operation aborted";
            }
            return null;
        }
    }
}
