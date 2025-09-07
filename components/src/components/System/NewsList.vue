<template>
    <div class="block rounded-4 mt-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`" alt="logo">
                    <span>列表</span>
                </h4>
            </div>
            <a class="btn btn-teal-dark" href="NewsEdit.aspx">
                <i class="fa-solid fa-plus"></i>
                新增公告
            </a>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table" aria-label="公告列表">
                <thead>
                    <tr>
                        <th scope="col" style="width:220px">公告期間</th>
                        <th class="text-start" scope="col">公告標題</th>
                        <th class="text-start" scope="col" style="width:200px">發布者</th>
                        <th scope="col" style="width:180px">功能</th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="公告期間:">
                            <tw-date :value="item.EnableTime"></tw-date>
                            ~
                            <tw-date :value="item.DisableTime" v-if="item.DisableTime"></tw-date>
                            <span v-else>未設定</span>
                        </td>
                        <td data-th="公告標題:" class="text-start">{{ item.Title }}</td>
                        <td data-th="發布者:" class="text-start">{{ item.UserOrg }} {{ item.UserName }}</td>
                        <td data-th="功能:" class="text-center">
                            <div class="d-inline-flex gap-2">
                                <a class="btn btn-sm btn-teal-dark" :href="`NewsEdit.aspx?id=${item.ID}`"><i class="fa-solid fa-pen"></i></a>
                                <button class="btn btn-sm btn-teal-dark" @click="remove(item)" type="button"><i class="fa-solid fa-xmark"></i></button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="list.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
    <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="modal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="fs-24 fw-bold text-green-light">確定刪除公告資料？</h4>
                    <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                        <i class="fa-solid fa-circle-xmark"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="d-flex gap-4 flex-wrap justify-content-center mt-5">
                        <button class="btn btn-gray" data-bs-dismiss="modal" type="button">取消</button>
                        <button class="btn btn-teal" @click="remove(target, true)" type="button">確認刪除</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    let target;

    const list = ref([]);
    const modal = ref();
    const page = ref(1);
    const size = ref(10);

    const rows = computed(() => list.value.slice((page.value - 1) * size.value, page.value * size.value));

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const remove = (item, confirm) => {
        if (!confirm) {
            target = item;
            bootstrap.Modal.getOrCreateInstance(modal.value).show();
        }

        api.system("deleteNews", { ID: item.ID }).subscribe((res) => {
            if (res) {
                list.value = list.value.filter((x) => x !== item);

                bootstrap.Modal.getOrCreateInstance(modal.value).hide();
            }
        });
    };

    onMounted(() => {
        api.system("getNewsList").subscribe((res) => list.value = res.List);
    });
</script>
