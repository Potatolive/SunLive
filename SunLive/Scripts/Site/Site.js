
/* -------------------- Isotope --------------------- */

jQuery(document).ready(function () {

    $('#wall').imagesLoaded(function () {

        var $container = $('#wall');
        $select = $('#filters select');

        // initialize Isotope
        $container.isotope({
            // options...
            resizable: false, // disable normal resizing
            // set columnWidth to a percentage of container width
            masonry: { columnWidth: $container.width() / 12 }
        });

        // update columnWidth on window resize
        $(window).smartresize(function () {

            $container.isotope({
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

});
