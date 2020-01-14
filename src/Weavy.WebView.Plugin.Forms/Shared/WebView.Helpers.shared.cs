namespace Weavy.WebView.Plugin.Forms.Helpers
{
    /// <summary>
    /// Client script helper
    /// </summary>
    public static class ScriptHelper
    {        
        /// <summary>
        /// Connect to real time hub. Useful when the app has been inactive for a while and the real time hub is disconnected.
        /// </summary>
        public static string ConnectScript = @"
/********************************************/
/* Reconnect to weavy rtm or reload page      */
/********************************************/
try{  
    /* wvy.connection.connect();  */
} catch(e){}
";
        /// <summary>
        /// Check for notification badge updates. This will trigger the BadgeUpdated event
        /// </summary>
        public static string UpdateBadgeScript = @"
/********************************************/
/* update badge                             */
/********************************************/
try{ 
    weavyAppScripts.badge.check();
} catch(e){}
";
        /// <summary>
        /// Scripts injected in the web page
        /// </summary>
        public static string Scripts = @"

if(typeof weavyAppScripts === 'undefined') {

    var weavyAppScripts = weavyAppScripts || {};

    weavyAppScripts.ready = (function(){    
        document.addEventListener('turbolinks:load', function (e) { 
            Native('readyCallback', true);
        });
    })();


    weavyAppScripts.user = (function(){    
        function get(){            
            $.ajax('/a/users/me').then(function(response){                                   
                Native('userCallback', response);
            });   
        }

        return {
            get: get
        };
    })();

    /********************************************/
    /* Register user for azure notification hub */
    /********************************************/
    weavyAppScripts.push = (function(){    
        function register(){
            var userId = $('body').data('user');        
            var userGuid = $('body').data('guid');
        
            if(userId != -1){ 
                Native('registerForNotificationsCallback', 'uid:' + userGuid);
            }
        
        }

        document.addEventListener('turbolinks:load', function (e) { 
            register();
        });

        register();
    })();



    /********************************************/
    /* Handle badge changes                     */
    /********************************************/
    weavyAppScripts.badge = (function(){    
        wvy.connection.on('badge.weavy', function(e, data){
            Native('badgeCallback', data);
        }); 
        
        var check = function(){

            var checkNotifications = $.ajax('/a/notifications/unread');
            var checkConversations = $.ajax('/a/conversations/unread?followed=true');      
            var conversationCount = 0;
            var notificationCount = 0;
        
            $.when(checkNotifications, checkConversations).then(function(p1, p2){
                if(p1){
                    var response = p1[0];
                    notificationCount = response.data != null ? response.data.length : 0;                                   
                }

                if(p2){
                    var response = p2[0];
                    conversationCount = response.data != null ? response.data.length : 0;                                                   
                }

                Native('badgeCallback', {conversations: conversationCount, notifications: notificationCount});
            })
        };

        check();

        return {
            check: check
        };
    })();


    /********************************************/
    /* Handle theme                             */
    /********************************************/
    weavyAppScripts.theme = (function(){    
        function set(){
            $.ajax('/a/theme').then(function(response){
                Native('themeCallback', response);
            }).fail(function(e){
                Native('themeCallback', {});
            });
        }

        document.addEventListener('turbolinks:load', function (e) { 
            set();
        });

        set();
    })();


    /********************************************/
    /* Sign-in and sign-out                     */
    /********************************************/
    weavyAppScripts.authentication = (function(){      

        var signOut = function(){
            $(document).on('click', 'a[href^=""/sign-out""]', function(){       
    	        Native('signOutCallback', '');
                return true;
            });
        };
    
        signOut();

    })();



    /********************************************/
    /* Handle external links                    */
    /********************************************/
    weavyAppScripts.links = (function(){    

        function handle(){

           $(document).on('click', 'a[href^=http]', function (e) {

	            var url = $(this).attr('href');
                var target = $(this).attr('target');

	            if(url.indexOf(window.location.origin) == -1 || target == '_blank'){
			        e.preventDefault();	
                    Native('linkCallback', { url: url });
	            }
            });

            $(document).on('click', 'a[href^=ms-]', function (e) {

	            e.preventDefault();		
                var url = $(this).attr('href');
                Native('linkCallback', { url: url });	
            });
        }

        handle();
    })();


}



";
    }
}
