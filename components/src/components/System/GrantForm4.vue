<template>
    <div class="block">
        <div class="mt-4">
            <div class="table-responsive">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th><required-label>審查組別</required-label></th>
                            <td>
                                <div class="input-group" style="width:400px">
                                    <input-select :options="groups" text-name="Text" value-name="Value" v-model="group"></input-select>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <th>審查委員名單</th>
                            <td>
                                <ul class="teal-dark-tabs">
                                    <li :class="{ active: tab === 1 }">
                                        <a class="tab-link" @click="tab = 1" href="javascript:void(0)">逐筆輸入</a>
                                    </li>
                                    <li :class="{ active: tab === 2 }">
                                        <a class="tab-link" @click="tab = 2" href="javascript:void(0)">批次輸入</a>
                                    </li>
                                </ul>
                                <div class="table-responsive sub-table" v-if="tab === 1">
                                    <table class="table align-middle gray-table">
                                        <thead>
                                            <tr>
                                                <th width="1%"></th>
                                                <th>姓名</th>
                                                <th>Email</th>
                                                <th width="1%"></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr :key="item" v-for="(item,idx) in list[group]?.filter((item) => !item.Deleted)">
                                                <td>{{ idx + 1 }}</td>
                                                <td>
                                                    <input-text :error="errors[`item-${idx}-CommitteeUser`]" v-model.trim="item.CommitteeUser"></input-text>
                                                </td>
                                                <td>
                                                    <input-text :error="errors[`item-${idx}-Email`]" v-model.trim="item.Email"></input-text>
                                                </td>
                                                <td>
                                                    <div class="d-flex gap-2">
                                                        <button class="btn btn-sm btn-teal-dark" @click="item.Deleted = true" type="button"><i class="fas fa-trash-alt"></i></button>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <button class="btn btn-sm btn-teal" @click="list[group]?.push({})" type="button"><i class="fas fa-plus"></i>新增</button>
                                </div>
                                <div v-if="tab === 2">
                                    <div class="d-flex gap-2">
                                        <input-textarea :placeholder="`張曉明 <aming@gmail.com>\n陳小華 <hwahchen@xxx.edu.tw>`" rows="5" v-model.trim="text"></input-textarea>
                                        <button class="btn btn-teal-dark" @click="add" type="button"><i class="fa-solid fa-plus"></i>新增</button>
                                    </div>
                                    <label class="form-text">輸入格式範例「姓名 &lt;xxx@email.com&gt;」，多筆請斷行輸入</label>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="d-flex gap-3 flex-wrap justify-content-center mt-4">
            <button class="btn btn-outline-teal" @click="save" type="button">儲存</button>
        </div>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const errors = ref({});
    const group = ref();
    const groups = ref([]);
    const list = ref({});
    const tab = ref(1);
    const text = ref("");

    const add = () => {
        const regex = /^(.*?)\s*<([^>]+)>$/;

        text.value.split("\n").forEach((item) => {
            const match = item.trim().match(regex);

            if (match) {
                list.value[group.value].push({ CommitteeUser: match[1], Email: match[2] });
            }
        });

        tab.value = 1;
        text.value = "";
    };

    const emit = defineEmits(["next"]);

    const save = () => {
        if (!verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        api.system("saveReviewCommitteeList", { ID: group.value, List: list.value[group.value] }).subscribe((res) => {
            if (res) {
                emit("next");
            }
        });
    };

    const verify = () => {
        const rules = {
            CommitteeUser: "姓名",
            Email: { presence: { message: "^請輸入 Email" }, email: { message: "格式錯誤" } }
        };

        errors.value = {};

        list.value[group.value].filter((item) => !item.Deleted).forEach((item, idx) => Object.assign(errors.value, validateData(item, rules, `item-${idx}-`)));

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        api.system("getReviewGroups", { ID: props.id }).subscribe((res) => {
            groups.value = res.List;
            group.value = groups.value[0].Value;
        });
    });

    watch(group, (ID) => {
        if (!list.value[ID]) {
            api.system("getReviewCommitteeList", { ID }).subscribe((res) => {
                list.value[ID] = res.List;
            });
        }
    });
</script>
