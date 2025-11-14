<template>
    <div class="search bg-gray mt-4">
        <h3 class="text-teal">
            <i class="fa-solid fa-magnifying-glass"></i>
            查詢
        </h3>
        <div class="search-form">
            <div class="row g-3">
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">審查階段</div>
                    <input-select :empty-value="0" :options="types" v-model="form.Type"></input-select>
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">委員姓名, Email</div>
                    <input-text placeholder="請輸入委員姓名, Email" v-model.trim="form.Keyword"></input-text>
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">開始時間</div>
                    <input-tw-date v-model="form.Begin"></input-tw-date>
                </div>
                <div class="col-3">
                    <div class="fs-16 text-gray mb-2">結束時間</div>
                    <input-tw-date v-model="form.End"></input-tw-date>
                </div>
            </div>
            <button class="btn btn-teal-dark d-table mx-auto" @click="submit" type="button">
                <i class="fa-solid fa-magnifying-glass"></i>
                查詢
            </button>
        </div>
    </div>
    <div class="block rounded-bottom-4">
        <div class="title border-teal">
            <div class="d-flex align-items-center gap-2">
                <h4 class="text-teal">
                    <img :src="`../../assets/img/title-icon02-teal.svg`">
                    <span>列表</span>
                </h4>
                <span>共 <span class="text-teal">{{ list.length }}</span> 筆資料</span>
            </div>
            <button class="btn btn-teal-dark" @click="download" type="button"><i class="fas fa-download"></i>匯出</button>
        </div>
        <div class="table-responsive" style="min-height:400px">
            <table class="table teal-table">
                <thead>
                    <tr>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>委員姓名</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>Email</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>審查階段</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-center">
                                <span>審查計畫件數</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>銀行帳戶</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>戶籍地址</span>
                            </div>
                        </th>
                        <th>
                            <div class="hstack align-items-center justify-content-start">
                                <span>更新時間</span>
                            </div>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr :key="item" v-for="(item) in rows">
                        <td data-th="委員姓名:">{{ item.CommitteeUser }}</td>
                        <td data-th="Email:" class="text-start">{{ item.Email }}</td>
                        <td data-th="審查階段:">{{ item.Type === 1 ? "申請計畫" : "執行計畫" }}</td>
                        <td data-th="審查計畫件數:">
                            <a class="link-black" :href="`ReviewerList${item.Type}.aspx?ID=${item.ID}`" target="_blank">{{ item.Count }}</a>
                        </td>
                        <td data-th="銀行帳戶:" class="text-start">
                            <a @click.prevent="showPhoto(item)" href="#" v-if="item.BankPhoto">{{ item.BankCode }} {{ item.BankName }} {{ item.BankAccount }}</a>
                            <span v-else>{{ item.BankCode }} {{ item.BankName }} {{ item.BankAccount }}</span>
                        </td>
                        <td data-th="戶籍地址:" class="text-start">{{ item.RegistrationAddress }}</td>
                        <td data-th="更新時間:" class="text-start"><tw-date-time :value="item.UpdateTime"></tw-date-time></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <list-pager :count="list.length" @change="change" :page="page" :page-size="size"></list-pager>
    </div>
    <teleport to="body">
        <div aria-hidden="true" class="modal fade" ref="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-5">
                            <img class="img-fluid" :src="photo" />
                        </div>
                        <div class="d-flex justify-content-center">
                            <button class="btn btn-primary" data-bs-dismiss="modal" type="button">
                                確認
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </teleport>
</template>

<script setup>
    const form = ref({ Type: 0, Keyword: "", Begin: "", End: "" });
    const list = ref([]);
    const modal = ref();
    const page = ref(1);
    const size = ref(10);
    const photo = ref({});
    const types = [{ text: "申請計畫審查", value: 1 }, { text: "執行計畫審查", value: 2 }];

    const rows = computed(() => list.value.slice((page.value - 1) * size.value, page.value * size.value));

    const change = (to, newSize) => {
        page.value = parseInt(to);
        size.value = parseInt(newSize);
    };

    const download = () => {
        api.system("downloadReviewerList", form.value).subscribe((res) => {
            const link = document.createElement("a");

            link.download = "審查委員名單.xlsx";
            link.href = api.download(res.filename);
            link.click();
        });
    };

    const showPhoto = (item) => {
        photo.value = item.Type === 1 ? api.download(item.BankPhoto) : api.sciDownload("downloadbankbook", { token: item.BankPhoto?.trim() });

        bootstrap.Modal.getOrCreateInstance(modal.value).show();
    };

    const submit = () => {
        api.system("getReviewerList", form.value).subscribe((res) => {
            list.value = res.List;
            page.value = 1;
        });
    };

    onMounted(submit);
</script>
