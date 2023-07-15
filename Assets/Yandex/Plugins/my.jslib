mergeInto(LibraryManager.library, {

  	RateGame: function () {
    
    	ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                ysdk.feedback.requestReview()
                    .then(({ feedbackSent }) => {
                        console.log(feedbackSent);
                    })
            } else {
                console.log(reason)
            }
        })
  	},


	ShowAdv: function() {
		ysdk.adv.showFullscreenAdv({
    callbacks: {
        onClose: function(wasShown) {
		console.log("--------closed--------");
          // some action after close
        },
        onError: function(error) {
          // some action on error
        }
    }
})
	},

    GetLang: function () { 
        var lang = ysdk.environment.i18n.lang;
        var bufferSize = lengthBytesUTF8(lang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(lang, buffer, bufferSize);
        return buffer;
    },

    

    Auth: function () {
        initPlayer().then(_player => {
            if (_player.getMode() === 'lite') {
                // Игрок не авторизован.
                ysdk.auth.openAuthDialog().then(() => {
                    // Игрок успешно авторизован
                    initPlayer().catch(err => {
                        // Ошибка при инициализации объекта Player.
                    });
                }).catch(() => {
                    // Игрок не авторизован.
                });
            }
        }).catch(err => {
            // Ошибка при инициализации объекта Player.
        });
    }

  });