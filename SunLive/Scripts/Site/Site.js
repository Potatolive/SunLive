
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
        $('#imageWidth_' + id).val($('#img_' + id).width());
        $('#imageHeight_' + id).val($('#img_' + id).height());

        $.ajax({
            type: "POST",
            url: "../../Post/Crop",
            data: $('#form_crop_image_' + id).serialize(),
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                //if (jcrop_api) jcrop_api.release();
                
                var portrait = false;
                if ($('#img_' + id).height() > $('#img_' + id).width())
                {
                    portrait = true;
                }
                $('#item_' + id).find('img').attr('src', msg + '?' + Math.random());

                //$('#item_' + id).replaceWith($.parseHTML(msg));
                var $itemContainer = $('#item_' + id);

                //$container.find('img').css('height', '100px');
                //$container.find('.jcrop-holder').css('height', '100px');

                //$container.find('.jcrop-holder').css('width', 'auto');
                //$container.find('.jcrop-holder').css('height', 'auto');

                //$container.find('.jcrop-tracker').css('width', 'auto');
                //$container.find('.jcrop-tracker').css('height', 'auto');

                $itemContainer.find('.jcrop-holder').css('display', 'none');

                var $originalImage = $('#img_' + id);
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


                $('#revert_' + id).removeClass('hidden');
                $('#crop_' + id).addClass('hidden');
                console.log("success");

                
                //var $isoGrid = $('#wall').isotope('reloadItems').isotope({
                //    masonry: { columnWidth: $('#wall').width() / 12 }
                //});

                ////$('#wall').isotope('destroy');

                ////$isoGrid.isotope('layout');
                ////$isoGrid.isotope('remove', $('#item_' + id));
                //var newDiv = $('#wall').append(msg);
                
                //$('#wall').isotope('reloadItems').isotope('reLayout')
                //$isoGrid.isotope('append', newDiv, function () {
                //    console.log('inserted');
                //});

                //$isoGrid.isotope('remove', $('#item_' + id));
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
            url: "../Post/RevertCrop",
            data: $('#form_crop_image_' + id).serialize(),
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                var $itemContainer = $('#item_' + id);

                $itemContainer.find('img').attr('src', msg);
                $('#revert_' + id).addClass('hidden');
                $('#crop_' + id).removeClass('hidden');

                $itemContainer.find('.jcrop-holder').css('display', '');

                var $originalImage = $('#img_' + id);
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

        $.ajax({
            type: "GET",
            url: "../Post/Reject/" + id,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                if (msg == 'True') {
                    $('#item_' + id).removeClass('New').addClass('Rejected');
                    var $itemContainer = $('#item_' + id);

                    //Hide Cross Hair
                    $itemContainer.find('.jcrop-holder').css('display', 'none');

                    //Show original Image
                    var $originalImage = $('#img_' + id);
                    $originalImage.css('display', 'block');
                    $originalImage.css('visibility', 'visible');

                    //Hide buttons
                    $itemContainer.find('.rejectButton').addClass('hidden');
                    $itemContainer.find('.approveButton').addClass('hidden');
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

        $.ajax({
            type: "GET",
            url: "../Post/Approve/" + id,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                if (msg == 'True') {
                    $('#item_' + id).removeClass('New').addClass('Approved');

                    var $itemContainer = $('#item_' + id);

                    //Hide Cross Hair
                    $itemContainer.find('.jcrop-holder').css('display', 'none');

                    //Show original Image
                    var $originalImage = $('#img_' + id);
                    $originalImage.css('display', 'block');
                    $originalImage.css('visibility', 'visible');

                    //Hide buttons
                    $itemContainer.find('.rejectButton').addClass('hidden');
                    $itemContainer.find('.approveButton').addClass('hidden');
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

        
        $.ajax({
            type: "GET",
            url: "../Post/Delete/" + id,
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {

                if (msg == 'True') {
                    $('#wall').isotope('remove', $('#item_' + id));
                    console.log(msg);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    $('.image.New').mouseover(function () {

        var id = $(this).attr('imageId');
        console.log(id);

        $('#img_' + id).Jcrop({
            aspectRatio: 1,
            onSelect: function (c) {
                $('#X_' + id).val(c.x);
                $('#Y_' + id).val(c.y);
                $('#W_' + id).val(c.w);
                $('#H_' + id).val(c.h);
                $('#crop_' + id).removeClass('hidden');
            },
            onChange: function (c) {

            }
        });
    });


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
