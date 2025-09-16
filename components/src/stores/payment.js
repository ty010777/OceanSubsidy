export const usePaymentStore = defineStore("payment", {
    actions: {
        init(payment, approved = 0) {
            this.stage = payment?.Stage;
            this.amount = payment?.CurrentRequestAmount;
            this.paid = approved;
            this.status = payment?.Status;
        }
    },
    state: () => ({
        stage: null,
        amount: null,
        paid: null,
        status: null
    })
});
