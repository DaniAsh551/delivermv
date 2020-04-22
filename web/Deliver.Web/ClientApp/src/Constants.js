export default {
    APIBASE: '/api',
    PaymentMethods: {
        getDefinition: function(){
            return Array(5).fill(null).map((_,i) => ({ value:i, label:this.getText(i) }));
        },
        getText: function (id) {
            switch (id) {
                case 0:
                    return "Cash On Delivery";
                case 1:
                    return "BML Transfer";
                case 2:
                    return "MIB Transfer";
                case 3:
                    return "BML Card";
                case 4:
                    return "MIB Card";
            }
        },
        getClasses: function (id) {
            switch (id) {
                case 0:
                    return "badge badge-secondary";
                case 1:
                    return "badge badge-danger";
                case 2:
                    return "badge badge-success";
                case 3:
                    return "badge badge-danger";
                case 4:
                    return "badge badge-success";
                default:
                    return "d-none";
            }
        }
    },
    OrderStatuses: {
        getDefinition: function(id = null){
            const definition = [
            { label: 'Received' , value: 0},
            { label: 'Confirmed', value: 1},
            { label: 'Rejected' , value: 2},
            { label: 'Completed', value: 3}
            ];

            if(id)
                return definition[id];
            return definition;
        },
        getClasses: function(id){
            switch (id) {
                case 0:
                    return "bg-warning";
                case 1:
                    return "bg-primary";
                case 2:
                    return "bg-danger";
                case 3:
                    return "bg-success";
                default:
                    return "bg-secondary";
            }
        }
    }
};