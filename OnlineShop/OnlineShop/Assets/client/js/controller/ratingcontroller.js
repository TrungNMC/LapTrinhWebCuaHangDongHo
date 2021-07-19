var rate = {
    init: function () {
        rate.regEvents();
    },
    regEvents: function () {
        $('#btnsave').off('click').on('click', function () {
            var sao = "";
            var comment = $('#review').val();

            $.each($(".txtsao"), function (k, v) {
                if ($(v).prop('checked') == true) {

                    sao = $(v).val();

                }

            });

            if (sao.length > 0) {
                $.ajax({
                    url: "/Product/Rating",
                    data: { comment: comment, sao: sao },
                    success: function (data) {
                        //alert(status);

                        //  window.location = "/chi-tiet/" + data.MetaTitle + "-" + data.id;
                    }
                });
            }
        });
    }
}
rate.init();