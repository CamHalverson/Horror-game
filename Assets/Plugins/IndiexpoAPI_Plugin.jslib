mergeInto(LibraryManager.library, {
 
  openPage: function (url) {
    url = Pointer_stringify(url);
    console.log('Opening link: ' + url);
    window.open(url,'_blank');
  },

  ShowMessage: function (str) 
	{

    window.alert(Pointer_stringify(str));
	},


  UploadScore: function (s)
	{

		if (window.IndiexpoAPI)		
		{
		
				IndiexpoAPI.sendScore(s).done(function(result) {});
		}
		else
		{

				window.alert ("Please, login to indiexpo to send your score");		
		}		

	}, 


	CheckIfLogged: function()
	  {
	  
	  if (window.IndiexpoAPI)		
		{

			return true;		
		}
		else
		{

				
				return false;
		}		
	  
	  
	  
	  
	  },

  });