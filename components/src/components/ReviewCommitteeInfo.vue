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
                        <th><required-label>銀行帳戶</required-label></th>
                        <td>
                            <div class="d-flex gap-2">
                                <div>
                                    <input-select :empty-value="null" :error="errors.BankCode" :options="banks" placeholder="請選擇銀行" text-name="Descname" value-name="Code" v-model="form.BankCode"></input-select>
                                </div>
                                <div>
                                    <input-text :error="errors.BankAccount" placeholder="帳戶號碼" v-model.trim="form.BankAccount"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>存摺影本</required-label></th>
                        <td>
                            <div>
                                <input-file accept=".jpg,.png" @file="add"></input-file>
                                <img class="img-fluid mt-2" :src="download(form.BankPhoto)" v-if="form.BankPhoto" />
                            </div>
                            <div class="invalid mt-0" v-if="errors.BankPhoto">{{ errors.BankPhoto }}</div>
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
    const form = ref({});

    const add = (file) => {
        form.value.BankPhoto = file.Path;
    };

    const download = api.download;

    const save = () => {
        errors.value = {};

        if (!verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            Token: props.token,
            BankCode: form.value.BankCode,
            BankAccount: form.value.BankAccount,
            BankPhoto: form.value.BankPhoto,
            RegistrationAddress: form.value.RegistrationAddress
        };

        api.system("saveReviewCommittee", data).subscribe((res) => {
            if (res) {
                alert("儲存成功");
            }
        });
    };

    const verify = () => {
        let rules = {
            BankCode: "^請選擇銀行",
            BankAccount: "帳戶號碼",
            BankPhoto: "^請上傳存摺影本",
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
