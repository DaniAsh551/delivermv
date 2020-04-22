import http from '../services';

export default {
    searchStores: async function(search = null, island = null, page = 1) {
        return http.get(`/shops`, { search, island, page, pageSize:12 });
    },
    getIslands: async function() {
        return http.get('/islands', { pageSize:1500 });
    },
    getStore: async function (id) {
        return http.get(`/shops/${id}`);
    },
    createOrder: async function(order){
        return http.post('/orders', [order]);
    },
    getOrders: async function(search = null, paymentMethod = null, date = null, status = null, island = null,  page = 1){
        return http.get('/shops/orders', { search, paymentMethod, date, status, island, page, pageSize:12 });
    },
    getOrder: async function(id){
        return http.get(`/shops/orders/${id}`);
    },
    updateOrder: async function(id, { price, notes, status }){
        return http.post(`/shops/orders/${id}`, { notes, price, status });
    },
    trackOrder: async function(id){
        return http.get(`/orders/${id}`);
    }
};

