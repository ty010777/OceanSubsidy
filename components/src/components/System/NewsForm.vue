<template>
    <div class="block rounded-top-4 mt-4 d-flex flex-column gap-5">
        <div>
            <h5 class="square-title">公告內容</h5>
            <div class="table-responsive mt-3 mb-0">
                <table class="table align-middle gray-table side-table">
                    <tbody>
                        <tr>
                            <th>發佈者</th>
                            <td>{{ form.UserOrg }}&nbsp; &nbsp; {{ form.UserName }}</td>
                        </tr>
                        <tr>
                            <th><required-label>公告標題</required-label></th>
                            <td>
                                <input-text :error="errors.Title" v-model.trim="form.Title"></input-text>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>公告期間</required-label></th>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <div class="input-group w-auto">
                                        <input-tw-date class="w-auto" v-model="form.EnableTime"></input-tw-date>
                                        <span class="input-group-text">至</span>
                                        <input-tw-date class="w-auto" v-model="form.DisableTime"></input-tw-date>
                                    </div>
                                </div>
                                <div class="invalid mt-0" v-if="errors.EnableTime">{{ errors.EnableTime }}</div>
                            </td>
                        </tr>
                        <tr>
                            <th><required-label>內容</required-label></th>
                            <td>
                                <input-textarea :error="errors.Content" v-model.trim="form.Content"></input-textarea>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div>
            <div class="d-flex mb-2 gap-3">
                <h5 class="square-title">
                    <span>相關附件</span>
                </h5>
                <input-file accept=".pdf,.odt,.doc,.docx,.zip" @file="(file) => add(files, file)"></input-file>
            </div>
            <span class="text-pink">支援文件 pdf, odt, doc, docx (10MB以內)、ZIP 壓縮檔 (50MB以內)</span>
            <div class="tag-group mt-3">
                <template :key="file" v-for="(file) in files">
                    <span class="tag tag-green-light" v-if="!file.Deleted">
                        <a class="tag-link my-1" :download="file.Name" :href="download(file.Path)">{{ file.Name }}</a>
                        <button class="tag-btn" @click="file.Deleted = true" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </span>
                </template>
            </div>
        </div>
        <div>
            <div class="d-flex mb-2 gap-3">
                <h5 class="square-title">
                    <span>相關圖片</span>
                </h5>
                <input-file accept=".jpg,.png" @file="(file) => add(images, file)"></input-file>
            </div>
            <span class="text-pink">支援 JPG, PNG，檔案大小 10MB以內</span>
            <div class="image-preview mt-3">
                <template :key="file" v-for="(file) in images">
                    <div class="image-preview__item" v-if="!file.Deleted">
                        <img alt="picture" :src="download(file.Path)" />
                        <div class="image-preview__item-overlay">
                            <button aria-label="移除圖片" class="btn btn-remove-image" @click="file.Deleted = true" type="button">
                                <i class="fa-solid fa-xmark"></i>
                            </button>
                        </div>
                    </div>
                </template>
            </div>
        </div>
        <div>
            <div class="d-flex mb-2 gap-3">
                <h5 class="square-title">
                    <span>相關影片</span>
                </h5>
                <button aria-label="新增影片" class="btn btn-teal-dark" @click="videos.push({})" type="button">
                    <i class="fas fa-plus"></i>
                    新增
                </button>
            </div>
            <div class="table-responsive mt-3" v-if="filteredVideos.length">
                <table class="table align-middle gray-table">
                    <thead>
                        <tr>
                            <th width="80" class="text-center">項次</th>
                            <th>影片名稱</th>
                            <th>影片網址</th>
                            <th width="120">功能</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr :key="item" v-for="(item,idx) in filteredVideos">
                            <td class="text-center">{{ idx + 1 }}</td>
                            <td>
                                <input-text :error="errors[`video-${idx}-Title`]" v-model.trim="item.Title"></input-text>
                            </td>
                            <td>
                                <input-text :error="errors[`video-${idx}-URL`]" v-model.trim="item.URL"></input-text>
                            </td>
                            <td>
                                <div class="d-flex gap-1">
                                    <button aria-label="刪除影片" class="btn btn-sm btn-teal-dark" @click="item.Deleted = true" type="button">
                                        <i class="fas fa-trash-alt"></i>
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div class="block-bottom bg-light-teal">
        <button class="btn btn-teal" @click="save" type="button"><i class="fas fa-check"></i>確認送出</button>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const errors = ref({});
    const files = ref([]);
    const filteredVideos = computed(() => videos.value.filter((video) => !video.Deleted));
    const form = ref({});
    const images = ref([]);
    const videos = ref([]);

    const add = (items, file) => items.push({ Path: file.Path, Name: file.Name });

    const download = api.download;

    const emit = defineEmits(["next"]);

    const save = () => {
        if (!verify()) {
            nextTick(() => document.querySelector(".invalid")?.scrollIntoView({ behavior: "smooth", block: "center" }));
            return;
        }

        const data = {
            News: form.value,
            Files: files.value,
            Images: images.value,
            Videos: videos.value
        };

        api.system("saveNews", data).subscribe((res) => {
            if (res) {
                emit("next");
            }
        });
    };

    const verify = () => {
        let rules = {
            Title: "公告標題",
            EnableTime: "公告期間",
            Content: "內容"
        };

        errors.value = validateData(form.value, rules);

        rules = {
            Title: "影片名稱",
            URL: "影片網址"
        };

        filteredVideos.value.forEach((video, idx) => Object.assign(errors.value, validateData(video, rules, `video-${idx}-`)));

        return !Object.keys(errors.value).length;
    };

    onMounted(() => {
        if (props.id) {
            api.system("getNews", { ID: props.id }).subscribe((res) => {
                form.value = res.News;
                files.value = res.Files;
                images.value = res.Images;
                videos.value = res.Videos;
            });
        } else {
            api.system("getEmptyNews").subscribe((res) => form.value = res);
        }
    });
</script>
