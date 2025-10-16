<template>
    <div class="block">
        <h5 class="square-title">委員資料</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>審查委員</th>
                        <td>
                            {{ form.CommitteeUser }} ({{ form.Email }})
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位類別</required-label></th>
                        <td>
                            <div class="d-flex gap-2">
                                <div>
                                    <input-select empty-value="" :error="errors.BankCode" :options="banks" placeholder="請選擇銀行" text-name="Descname" value-name="Code" v-model="form.BankCode"></input-select>
                                </div>
                                <div>
                                    <input-text :error="errors.BankAccount" placeholder="帳戶號碼" v-model.trim="form.BankAccount"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>戶籍地址</required-label></th>
                        <td>
                            <input-text :error="errors.RegistrationAddress" placeholder="(郵遞區號)完整地址" v-model.trim="form.RegistrationAddress"></input-text>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="block-bottom bg-light-teal">
        <button class="btn btn-teal" @click="save" type="button"><i class="fas fa-check"></i>確定送出</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        token: { type: String }
    });

    const banks = ref([]);
    const errors = ref({});
    const form = ref({ BankCode: "" });

    const save = () => {
        errors.value = {};

        if (!verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        api.system("saveReviewCommittee", { Token: props.token, BankCode: form.value.BankCode, BankAccount: form.value.BankAccount, RegistrationAddress: form.value.RegistrationAddress }).subscribe((res) => {
            alert("儲存成功");
        });
    };

    const verify = () => {
        let rules = {
            BankCode: "銀行",
            BankAccount: "帳戶號碼",
            RegistrationAddress: "戶籍地址"
        };

        errors.value = validateData(form.value, rules);

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.system("getZgsCodes", { CodeGroup: "BankCode" }),
            api.system("getReviewCommittee", { Token: props.token })
        ]).subscribe((result) => {
            banks.value = result[0];
            form.value = result[1].Data;
        });
    });
</script>
