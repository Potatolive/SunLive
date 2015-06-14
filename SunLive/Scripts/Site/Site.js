
/* -------------------- Isotope --------------------- */

var $grid;
jQuery(document).ready(function () {

    $('#wall').imagesLoaded(function () {

        var $container = $('#wall');
        $select = $('#filters select');

        // initialize Isotope
        $grid = $container.isotope({
            // options...
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
                // update columnWidth to a percentage of container width
                masonry: { columnWidth: $container.width() / 12 }
            });
        });


        $container.isotope({
            itemSelector: '.item'
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

        id = $(this).attr('imaageId');
        $('#imageWidth_' + id).val($('#img_' + id).width());
        $('#imageHeight_' + id).val($('#img_' + id).height());

        $.ajax({
            type: "POST",
            url: "../Post/Crop",
            data: $('#form_crop_image_' + id).serialize(),
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (msg) {
                //if (jcrop_api) jcrop_api.release();
                var test = 'http://localhost:54442/output/a22e0c4b-f479-4f35-a426-4050f2ada27e.jpg';
                //$('#item_' + id).find('img').attr('src', test + '?' + Math.random());

                //$('#item_' + id).replaceWith($.parseHTML(msg));
                //var $container = $('#item_' + id);

                //$container.find('img').css('height', '100px');
                //$container.find('.jcrop-holder').css('height', '100px');

                //$container.find('.jcrop-holder').css('width', 'auto');
                //$container.find('.jcrop-holder').css('height', 'auto');

                ////$container.find('.jcrop-tracker').css('width', 'auto');
                ////$container.find('.jcrop-tracker').css('height', 'auto');

                //$container.find('img').css('width', 'auto');
                //$container.find('img').css('height', 'auto');


                $('#revert_' + id).removeClass('hidden');
                $('#crop_' + id).addClass('hidden');
                console.log("success");

                
                var $isoGrid = $('#wall').isotope('reloadItems').isotope({
                    itemClass: 'item',
                    masonry: { columnWidth: $('#wall').width() / 12 }
                });

                //$('#wall').isotope('destroy');

                //$isoGrid.isotope('layout');
                $isoGrid.isotope('remove', $('#item_' + id));
                $isoGrid.isotope('appended', $.parseHTML(msg), function () {
                    console.log('inserted');
                });

                //$isoGrid.isotope('remove', $('#item_' + id));
                //$isoGrid.isotope('layout');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    });

    

});


function revertCrop(e, id) {
    $.ajax({
        type: "POST",
        url: "Home/RevertCrop",
        data: $('#form_crop_image_' + id).serialize(),
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        success: function (msg) {
            $('#img_' + id).find('img').attr('src', msg);
            $('#revert_' + id).addClass('hidden');
            $('#crop_' + id).addClass('hidden');
            console.log(msg);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

function cropImage(id) {
    var jcrop_api;

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

   
}
