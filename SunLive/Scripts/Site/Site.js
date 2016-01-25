
/* -------------------- Isotope --------------------- */

var $grid;
jQuery(document).ready(function () {

    $('#wall').imagesLoaded(function () {

        var $container = $('#wall');
        $select = $('#filters select');

        // initialize Isotope
        $grid = $container.isotope({
            // options...
            itemSelector: '.grid-item',
            resizable: false, // disable normal resizing
            // set columnWidth to a percentage of container width
            masonry: { columnWidth: $container.width() / 12 },
            //sortBy: 'number',
            //getSortData: {
            //    'number': '.number parseInt'
            //}
        });

        // update columnWidth on window resize
        $(window).smartresize(function () {

            $grid = $container.isotope({
                itemSelector: '.grid-item',
                // update columnWidth to a percentage of container width
                masonry: { columnWidth: $container.width() / 12 }
            });
        });

    
        $select.change(function () {

            var filters = $(this).val();

            $container.isotope({
                filter: filters
            });

        });

        var $optionSets = $('#filters .option-set'),
        $optionLinks = $optionSets.find('a');

        $optionLinks.click(function () {

            var $this = $(this);
            // don't proceed if already selected
            if ($this.hasClass('selected')) {
                return false;
            }
            var $optionSet = $this.parents('.option-set');
            $optionSet.find('.selected').removeClass('selected');
            $this.addClass('selected');

            // make option object dynamically, i.e. { filter: '.my-filter-class' }
            var options = {},
				key = $optionSet.attr('data-option-key'),
				value = $this.attr('data-option-value');
            // parse 'false' as false boolean
            value = value === 'false' ? false : value;
            options[key] = value;
            if (key === 'layoutMode' && typeof changeLayoutMode === 'function') {
                // changes in layout modes need extra logic
                changeLayoutMode($this, options)
            } else {
                // otherwise, apply new options
                $container.isotope(options);
            }

            return false;

        });

       

    });


    $('.cropButton').click(function () {

        var id = $(this).attr('data-imageid');
        $('[id="imageWidth_' + id + '"]').val($('[id="img_' + id + '"]').width());
        $('[id="imageHeight_' + id + '"]').val($('[id="img_' + id + '"]').height());

        $.ajax({
            type: "POST",
            url: "../../Post/Crop",
            data: $('[id="form_crop_image_' + id + '"]').serialize(),
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                //if (jcrop_api) jcrop_api.release();
                
                var portrait = false;
                if ($('[id="img_' + id + '"]').height() > $('[id="img_' + id + '"]').width())
                {
                    portrait = true;
                }
                $('[id="item_' + id + '"]').find('img').attr('src', msg + '?' + Math.random());

                //$('#item_' + id).replaceWith($.parseHTML(msg));
                var $itemContainer = $('[id="item_' + id + '"]');

                $itemContainer.find('.jcrop-holder').css('display', 'none');

                var $originalImage = $('[id="img_' + id + '"]');
                if (portrait == true) {
                    $originalImage.css('height', 'auto');
                }
                else
                {
                    $originalImage.css('width', 'auto');
                    $originalImage.css('margin', 'auto');
                }
                //$originalImage.css('width', '100%');
                $originalImage.css('display', 'block');
                $originalImage.css('visibility', 'visible');


                $('[id="revert_' + id + '"]').removeClass('hidden');
                $('[id="crop_' + id + '"]').addClass('hidden');
                console.log("success");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    $('.revertButton').click(function () {
        var id = $(this).attr('data-imageid');
        $.ajax({
            type: "POST",
            url: "../../Post/RevertCrop",
            data: $('[id="form_crop_image_' + id + '"]').serialize(),
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                var $itemContainer = $('[id="item_' + id + '"]');

                $itemContainer.find('img').attr('src', msg);
                $('[id="revert_' + id + '"]').addClass('hidden');
                $('[id="crop_' + id + '"]').removeClass('hidden');

                $itemContainer.find('.jcrop-holder').css('display', '');

                var $originalImage = $('[id="img_' + id + '"]');
                $originalImage.css('display', 'none');
                $originalImage.css('visibility', 'hidden');


                console.log(msg);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    $('.rejectButton').click(function () {
        var id = $(this).attr('data-imageid');
        console.log(id);

        var pageName = $(this).attr('data-pagename');

        $.ajax({
            type: "GET",
            url: "../../Post/Reject/?id=" + id + "&pageName=" + pageName,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                if (msg == 'True') {
                    $('[id="item_' + id + '"]').removeClass('New').addClass('Rejected');
                    var $itemContainer = $('[id="item_' + id + '"]');

                    //Hide Cross Hair
                    $itemContainer.find('.jcrop-holder').css('display', 'none');

                    //Show original Image
                    var $originalImage = $('[id="img_' + id + '"]');
                    $originalImage.css('display', 'block');
                    $originalImage.css('visibility', 'visible');

                    //Hide buttons
                    $itemContainer.find('.rejectButton').addClass('hidden');
                    $itemContainer.find('.approveButton').addClass('hidden');
                    $itemContainer.find('.prioritizeButton').addClass('hidden');
                    $itemContainer.find('.cropButton').addClass('hidden');
                    $itemContainer.find('.revertButton').addClass('hidden');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });
    
    $('.approveButton').click(function () {
        var id = $(this).attr('data-imageid');
        console.log(id);

        var pageName = $(this).attr('data-pagename');

        console.log(pageName);


        $.ajax({
            type: "GET",
            url: "../../Post/Approve/?id=" + id + "&pageName=" + pageName,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                if (msg == 'True') {
                    $('[id="item_' + id + '"]').removeClass('New').addClass('Approved');

                    var $itemContainer = $('[id="item_' + id + '"]');

                    //Hide Cross Hair
                    $itemContainer.find('.jcrop-holder').css('display', 'none');

                    //Show original Image
                    var $originalImage = $('[id="img_' + id + '"]');
                    $originalImage.css('display', 'block');
                    $originalImage.css('visibility', 'visible');

                    //Hide buttons
                    $itemContainer.find('.rejectButton').addClass('hidden');
                    $itemContainer.find('.approveButton').addClass('hidden');
                    $itemContainer.find('.prioritizeButton').addClass('hidden');
                    $itemContainer.find('.cropButton').addClass('hidden');
                    $itemContainer.find('.revertButton').addClass('hidden');

                    //Add Delete Class
                    $itemContainer.find('.deleteButton').removeClass('hidden');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });


    $('.prioritizeButton').click(function () {
        var id = $(this).attr('data-imageid');
        console.log(id);

        var pageName = $(this).attr('data-pagename');

        $.ajax({
            type: "GET",
            url: "../../Post/Prioritize/?id=" + id + "&pageName=" + pageName,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                if (msg == 'True') {
                    $('[id="item_' + id + '"]').removeClass('New').addClass('Approved');

                    var $itemContainer = $('[id="item_' + id + '"]');

                    //Hide Cross Hair
                    $itemContainer.find('.jcrop-holder').css('display', 'none');

                    //Show original Image
                    var $originalImage = $('[id="img_' + id + '"]');
                    $originalImage.css('display', 'block');
                    $originalImage.css('visibility', 'visible');

                    //Hide buttons
                    $itemContainer.find('.rejectButton').addClass('hidden');
                    $itemContainer.find('.approveButton').addClass('hidden');
                    $itemContainer.find('.prioritizeButton').addClass('hidden');
                    $itemContainer.find('.cropButton').addClass('hidden');
                    $itemContainer.find('.revertButton').addClass('hidden');

                    //Add Delete Class
                    $itemContainer.find('.deleteButton').removeClass('hidden');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    $('.deleteButton').click(function () {
        var id = $(this).attr('data-imageid');
        console.log(id);

        var pageName = $(this).attr('data-pagename');

        $.ajax({
            type: "GET",
            url: "../../Post/Delete/?id=" + id + "&pageName=" + pageName,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {

                if (msg == 'True') {
                    $('#wall').isotope('remove', $('[id="item_' + id + '"]'));
                    console.log(msg);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    $('.image').mouseover(function () {

        var id = $(this).attr('imageId');
        $('[id="img_' + id +'"]').Jcrop({
            aspectRatio: 1,
            onSelect: function (c) {
                $('[id="X_' + id + '"]').val(c.x);
                $('[id="Y_' + id + '"]').val(c.y);
                $('[id="W_' + id + '"]').val(c.w);
                $('[id="H_' + id + '"]').val(c.h);
                $('[id="crop_' + id + '"]').removeClass('hidden');
            },
            onChange: function (c) {

            }
        });
    });

    $('#refresh').click(function () {
        var pageName = $('#postcount').attr('data-pagename');
        var today = $('#postcount').attr('data-today');
        window.location.href = "../../Post/Index/?pageName=" + pageName + "&publishedOn=" + today;
    });

    if ($('#postcount')) {
        setInterval(function () {

            var pageName = $('#postcount').attr('data-pagename');
            var today = $('#postcount').attr('data-today');

            $.ajax({
                type: "GET",
                url: "../../Post/TodaysCount/?pageName=" + pageName + "&publishedOn=" + today,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (msg) {
                    var newMessage = msg - $('#postcount').val();
                    if (newMessage > 0) {
                        $('#newMessages .messageCount').text(newMessage + " new messages");
                        $('#newMessages').removeClass('hidden');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                }
            });
        }, 3000);
    }

});


function testing()
{
    $.ajax({
        type: "GET",
        url: "../post/partial/132507616819788_838388519588064",
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (msg) {
            $item = $('<div></div>');
            $item.html(msg);
            console.log($item.html());
            //$item = $('#item_132507616819788_838919079535008');
            //$item.attr('id', 'test');
            $('#wall').append($item).isotope('appended',$item);

            
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

function ClearWhatsApp() {
    $.ajax({
        type: "POST",
        url: "../ClearWhatsApp",
        data: {},
        success: function (msg) {
            $('.error').text('Fixed ...');
            $('.error').fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
            $('.error').text('Error. Call Support ...');
            $('.error').fadeIn(400).delay(3000).fadeOut(400); //fade out after 3 seconds
        }
    });
    
    $('.error').text('Send a dummy whatsapp message ...');
    $('.error').fadeIn(400).delay(10000).fadeOut(400); //fade out after 3 seconds
}