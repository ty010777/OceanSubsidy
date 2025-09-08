<template>
    <div class="block">
        <div class="mt-4">
            <div class="table-responsive">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th></th>
                            <th>適用申請單位類別</th>
                            <th>補助經費上限(萬)</th>
                            <th>配合款</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item,idx) in list">
                            <td>{{ idx + 1 }}</td>
                            <td>{{ item.TargetName }}</td>
                            <td>
                                <input-integer v-model="item.GrantLimit"></input-integer>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <input class="form-check-input" :id="`checkbox-${idx}`" type="checkbox" v-model="item.checked" />
                                    <label class="form-check-label ms-1" :for="`checkbox-${idx}`">配合款分攤比例</label>
                                    <template v-if="item.checked">
                                        <input-integer class="w-auto" v-model="item.MatchingFund"></input-integer>(%)
                                    </template>
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

    const list = ref([]);

    const emit = defineEmits(["next"]);

    const save = () => {
        const data = list.value.map((item) => ({ ...item, MatchingFund: item.checked ? item.MatchingFund : null }));

        api.system("saveGrantTargetSettings", { List: data }).subscribe((res) => {
            if (res) {
                emit("next");
            }
        });
    };

    onMounted(() => {
        api.system("getGrantTargetSettings", { ID: props.id }).subscribe((res) => {
            list.value = res.List;

            list.value.forEach((item) => {
                if (typeof item.MatchingFund === "number") {
                    item.checked = true;
                }
            });
        });
    });
</script>
