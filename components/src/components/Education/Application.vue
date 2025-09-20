<template>
    <div class="block">
        <h5 class="square-title">基本資料</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th>年度</th>
                        <td>
                            {{ form.Year }}
                        </td>
                    </tr>
                    <tr>
                        <th>計畫編號</th>
                        <td>
                            <span v-if="form.ProjectID">{{ form.ProjectID }}</span>
                            <span class="text-light-gray" v-else>儲存後由系統自動產生</span>
                        </td>
                    </tr>
                    <tr>
                        <th>補助計畫類別</th>
                        <td>
                            {{ form.SubsidyPlanType }}
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>計畫名稱</required-label></th>
                        <td>
                            <input-text :error="errors.ProjectName" v-model.trim="form.ProjectName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位類型</required-label></th>
                        <td>
                            <div class="d-flex flex-column gap-2">
                                <div>
                                    <input-select :empty-value="null" :error="errors.OrgCategory" :options="categories" text-name="Descname" value-name="Code" v-model="form.OrgCategory"></input-select>
                                </div>
                                <div v-if="form.OrgCategory === '2'">
                                    <input-text :error="errors.RegisteredNum" prefix="立案登記證字號" v-model.trim="form.RegisteredNum"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>申請單位</required-label></th>
                        <td>
                            <input-text :error="errors.OrgName" placeholder="請輸入名稱" v-model.trim="form.OrgName"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>統一編號</required-label></th>
                        <td>
                            <input-text :error="errors.TaxID" :max-length="8" v-model.trim="form.TaxID"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>地址</required-label></th>
                        <td>
                            <input-text :error="errors.Address" placeholder="請輸入地址" v-model.trim="form.Address"></input-text>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-5">人員聯絡資訊</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr :key="item" v-for="(item, idx) in contacts">
                        <th><required-label>{{ item.Role }}</required-label></th>
                        <td>
                            <div class="row g-3">
                                <div class="col-12 col-xl-2">
                                    <div class="text-gray mb-2"><required-label>姓名</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-Name`]" v-model.trim="item.Name"></input-text>
                                </div>
                                <div class="col-12 col-xl-3">
                                    <div class="text-gray mb-2"><required-label>職稱</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-JobTitle`]" v-model.trim="item.JobTitle"></input-text>
                                </div>
                                <div class="col-12 col-xl-4" v-if="idx">
                                    <div class="text-gray mb-2">電話(分機)</div>
                                    <div class="row g-1">
                                        <div class="col-7">
                                            <input-text v-model.trim="item.Phone"></input-text>
                                        </div>
                                        <div class="col-5">
                                            <input-text placeholder="請輸入分機" size="4" v-model.trim="item.PhoneExt"></input-text>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12 col-xl-3" v-if="idx">
                                    <div class="text-gray mb-2"><required-label>手機號碼</required-label></div>
                                    <input-text :error="errors[`contact-${idx}-MobilePhone`]" v-model.trim="item.MobilePhone"></input-text>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-5">計畫內容</h5>
        <div class="mt-4">
            <table class="table align-middle gray-table side-table">
                <tbody>
                    <tr>
                        <th><required-label>計畫期程</required-label></th>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <div class="input-group w-auto">
                                    <input-tw-date class="w-auto" v-model="form.StartTime"></input-tw-date>
                                    <span class="input-group-text">至</span>
                                    <input-tw-date class="w-auto" v-model="form.EndTime"></input-tw-date>
                                </div>
                                <span class="text-teal-dark">(計畫執行期間不得跨年度)</span>
                            </div>
                            <div class="invalid mt-0" v-if="errors.StartTime || errors.EndTime">{{ errors.StartTime || errors.EndTime }}</div>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>參加對象及人數</required-label></th>
                        <td>
                            <input-text :error="errors.Target" v-model.trim="form.Target"></input-text>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>計畫內容摘要</required-label></th>
                        <td>
                            <input-textarea :error="errors.Summary" :max-length="500" placeholder="請輸入計畫內容摘要" rows="4" v-model.trim="form.Summary"></input-textarea>
                        </td>
                    </tr>
                    <tr>
                        <th><required-label>預期效益<br />(建議條列式)</required-label></th>
                        <td>
                            <input-textarea :error="errors.Quantified" :max-length="500" placeholder="請輸入預期效益" rows="4" v-model.trim="form.Quantified"></input-textarea>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <h5 class="square-title mt-5">計畫經費</h5>
        <div class="mt-4">
            <ul class="list-unstyled lh-base mb-3">
                <li>其他單位及政府機關補助經費等欄，請詳實填寫；未接受補助者，請填寫0。經費單位為新臺幣(元)。</li>
            </ul>
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th class="text-end"><required-label>申請海委會<br />補助經費</required-label></th>
                            <th class="text-end"><required-label>申請單位<br />自籌款</required-label></th>
                            <th class="text-end"><required-label>其他政府機關<br />補助經費</required-label></th>
                            <th class="text-end"><required-label>其他單位<br />補助經費（含總收費）</required-label></th>
                            <th class="text-end">計畫總經費</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <input-integer :error="errors.ApplyAmount" v-model="form.ApplyAmount"></input-integer>
                            </td>
                            <td>
                                <input-integer :error="errors.SelfAmount" v-model="form.SelfAmount"></input-integer>
                            </td>
                            <td>
                                <input-integer :error="errors.OtherGovAmount" v-model="form.OtherGovAmount"></input-integer>
                            </td>
                            <td>
                                <input-integer :error="errors.OtherUnitAmount" v-model="form.OtherUnitAmount"></input-integer>
                            </td>
                            <td class="text-end">
                                {{ total.toLocaleString() }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <table class="table align-middle gray-table side-table">
            <tbody>
                <tr>
                    <th><required-label>最近兩年曾獲<br />本會補助</required-label></th>
                    <td>
                        <input-boolean v-model="hasReceiveds"></input-boolean>
                        <div class="table-responsive mt-3 mb-0" v-if="hasReceiveds">
                            <table class="table align-middle gray-table">
                                <thead>
                                    <tr>
                                        <th>計畫名稱</th>
                                        <th class="text-end">海委會補助經費</th>
                                        <th v-if="editable">功能</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr :key="item" v-for="(item, idx) in filteredReceiveds">
                                        <td>
                                            <input-text :error="errors[`received-${idx}-Name`]" v-model.trim="item.Name"></input-text>
                                        </td>
                                        <td>
                                            <input-integer :error="errors[`received-${idx}-Amount`]" v-model="item.Amount"></input-integer>
                                        </td>
                                        <td v-if="editable">
                                            <div class="d-flex gap-2">
                                                <button class="btn btn-sm btn-teal-dark m-0" @click="remove(item)" type="button" v-if="filteredReceiveds.length > 1"><i class="fas fa-trash-alt"></i></button>
                                                <button class="btn btn-sm btn-teal-dark m-0" @click="add" type="button" v-if="idx + 1 === filteredReceiveds.length"><i class="fas fa-plus"></i></button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <template v-if="changeForm">
            <h5 class="square-title mt-5">變更說明</h5>
            <div class="mt-4">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>變更前</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form1Before" rows="4" v-model.trim="changeForm.Form1Before"></input-textarea>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>變更後</required-label></th>
                            <td>
                                <input-textarea :error="errors.Form1After" rows="4" v-model.trim="changeForm.Form1After"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </template>
    </div>
    <div class="block-bottom bg-light-teal" v-if="editable">
        <button class="btn btn-outline-teal me-3" @click="save()" type="button">暫存</button>
        <button class="btn btn-teal" @click="save(true)" type="button"><i class="fas fa-check"></i>完成本頁，下一步</button>
    </div>
    <confirm-modal ref="modal">
        <div class="text-center">
            <div class="mb-3">［提醒您］</div>
            <div class="mb-5">您申請的補助款經費總計，已超過補助原則上限：{{ setting?.GrantLimit }}萬</div>
            <div class="mb-2">繼續申請請按【確定】</div>
            <div>返回調整請按【取消】</div>
        </div>
    </confirm-modal>
</template>

<script setup>
    const props = defineProps({
        apply: { default: false, type: Boolean },
        id: { type: [Number, String] }
    });

    const categories = ref([]);
    const changeForm = ref();
    const contacts = ref();
    const editable = computed(() => isProjectEditable("education", form.value.Status, 1));
    const errors = ref({});
    const filteredReceiveds = computed(() => receiveds.value.filter((item) => !item.Deleted));
    const form = ref({});
    const modal = ref();
    const receiveds = ref([]);
    const setting = ref();
    const total = ref(0);

    const hasReceiveds = computed({
        get: () => !!filteredReceiveds.value.length,
        set: (value) => {
            if (value) {
                add();
            } else {
                receiveds.value.forEach((item) => item.Deleted = true);
            }
        }
    });

    const add = () => receiveds.value.push({});

    const emit = defineEmits(["next"]);

    const load = () => {
        api.education("getApplication", { Apply: props.apply ? "true" : "false", ID: props.id }).subscribe((res) => {
            form.value = res.Project;
            contacts.value = res.Contacts;
            receiveds.value = res.ReceivedSubsidies;

            form.value.OrgCategory = form.value.OrgCategory || null;

            useProgressStore().init("education", form.value);

            changeForm.value = form.value.changeApply;
            form.value.changeApply = undefined;
        });
    };

    const remove = (target) => target.Deleted = true;

    const save = (submit, confirm) => {
        errors.value = {};

        if (submit && !verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        if (!confirm && setting.value && form.value.ApplyAmount > setting.value.GrantLimit * 10000) {
            modal.value.show(() => save(submit, true));
            return;
        }

        const data = {
            Project: form.value,
            Contacts: contacts.value,
            ReceivedSubsidies: receiveds.value.filter((item) => item.ID || !item.Deleted),
            Before: changeForm.value?.Form1Before,
            After: changeForm.value?.Form1After,
            Submit: submit ? "true" : "false"
        };

        api.education("saveApplication", data).subscribe((res) => {
            if (submit) {
                emit("next", res.ID);
            } else if (form.value.ID) {
                load();
            } else {
                window.location.href = `Application.aspx?ID=${res.ID}`;
            }
        });
    };

    const verify = () => {
        let rules = {
            ProjectName: "計畫名稱",
            OrgCategory: "申請單位類型",
            OrgName: "申請單位",
            TaxID: "統一編號",
            Address: "地址",
            StartTime: "計畫期程",
            EndTime: "計畫期程",
            Target: "參加對象及人數",
            Summary: "計畫內容摘要",
            Quantified: "預期效益",
            ApplyAmount: { presence: { allowEmpty: false, message: `^請輸入申請海委會補助經費` }, numericality: { greaterThan: 0, message: "^海委會補助經費不可為 0" } },
            SelfAmount: "申請單位自籌款",
            OtherGovAmount: "其他政府機關補助經費",
            OtherUnitAmount: "其他單位補助經費"
        };

        if (form.value.OrgCategory === "2") {
            rules.RegisteredNum = "立案登記證字號";
        }

        errors.value = validateData(form.value, rules);

        rules = {
            Name: "姓名",
            JobTitle: "職稱"
        };

        Object.assign(errors.value, validateData(contacts.value[0], rules, `contact-${0}-`))

        rules.MobilePhone = "手機號碼";

        Object.assign(errors.value, validateData(contacts.value[1], rules, `contact-${1}-`))

        rules = {
            Name: "計畫名稱",
            Amount: { presence: { allowEmpty: false, message: `^請輸入海委會補助經費` }, numericality: { greaterThan: 0, message: "^海委會補助經費不可為 0" } }
        };

        receiveds.value.forEach((received, index) => Object.assign(errors.value, validateData(received, rules, `received-${index}-`)));

        if (changeForm.value) {
            rules = {
                Form1Before: "變更前說明",
                Form1After: "變更後說明"
            };

            Object.assign(errors.value, validateData(changeForm.value, rules));
        }

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        rxjs.forkJoin([
            api.education("getZgsCodes", { CodeGroup: "EDCOrgCategory" })
        ]).subscribe((result) => {
            categories.value = result[0];

            if (props.id) {
                load();
            } else {
                api.education("getEmptyApplication").subscribe((res) => {
                    form.value = Object.assign({ OrgCategory: null, FormStep: 1, Status: 1 }, res);
                    contacts.value = [{ Role: "負責人" }, { Role: "聯絡人" }];

                    useProgressStore().init("education", form.value);
                });
            }
        });
    });

    provide("editable", editable);

    watch(() => form.value.OrgCategory, () => {
        api.education("getGrantTargetSetting", { TargetTypeID: `EDC${form.value.OrgCategory}` }).subscribe((res) => {
            setting.value = res;
        });
    });

    watchEffect(() => {
        total.value = toInt(form.value.ApplyAmount) + toInt(form.value.SelfAmount) + toInt(form.value.OtherGovAmount) + toInt(form.value.OtherUnitAmount);
    });
</script>
