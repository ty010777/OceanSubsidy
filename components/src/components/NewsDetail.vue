<template>
    <div class="block bg-light-teal-100 rounded-4 py-4">
        <h5 class="fw-bold">{{ form.Title }}</h5>
        <ul class="d-flex align-items-center gap-3 text-teal-dark mt-3">
            <li>公告日期：<tw-date :value="form.EnableTime"></tw-date></li>
            <li>公告單位：{{ form.UserOrg }}</li>
        </ul>
    </div>
    <div class="block rounded-4 mt-4">
        <div class="mb-5" style="white-space:pre-line" v-html="form.Content"></div>
        <template v-if="files.length">
            <h5 class="square-title">相關附件</h5>
            <ul class="mt-3 d-flex flex-column gap-2">
                <li :key="file" v-for="(file) in files">
                    <a class="link-black" :download="file.Name" :href="download(file.Path)">
                        {{ file.Name }}
                        <i class="fa-solid fa-file-arrow-down text-teal-dark ms-1"></i>
                    </a>
                </li>
            </ul>
        </template>
        <template v-if="images.length">
            <h5 class="square-title mt-5">相關照片</h5>
            <div class="mt-3">
                <div class="image-preview mt-3" ref="container">
                    <div class="image-preview__item" :key="file" v-for="(file, idx) in images">
                        <a data-fancybox="gallery" :data-caption="`相片 ${idx}`" :href="download(file.Path)">
                            <img :alt="`相片 ${idx}`" :src="download(file.Path)" />
                            <div class="image-preview__item-overlay">
                                <button aria-label="檢視圖片" class="btn btn-view-image" tabindex="-1" type="button">
                                    <i class="fa-solid fa-magnifying-glass"></i>
                                </button>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
        </template>
        <template v-if="videos.length">
            <h5 class="square-title mt-5">相關影片</h5>
            <ul class="mt-3 d-flex flex-column gap-2">
                <li :key="file" v-for="(file) in videos">
                    <a class="link-black" :href="file.URL" target="_blank">
                        {{ file.Title }}
                        <svg class="ms-1" xmlns="http://www.w3.org/2000/svg" width="16" height="18" viewBox="0 0 19 24" fill="none">
                            <g clip-path="url(#clip0_1051_6167)">
                                <path d="M17.7333 0.5H12.6667C11.913 0.5 11.4 1.12067 11.4 1.76667C11.4 2.05167 11.4887 2.36833 11.7673 2.65967L13.4077 4.3L9.87367 7.834C9.60767 8.1 9.5 8.423 9.5 8.727C9.5 9.43633 10.0573 9.99367 10.7667 9.99367C11.096 9.99367 11.4127 9.87333 11.6597 9.62L15.1937 6.086L16.834 7.72633C17.1253 8.005 17.442 8.09367 17.727 8.09367C18.373 8.09367 18.9937 7.58067 18.9937 6.827V1.76667C18.9937 1.12067 18.4807 0.5 17.727 0.5H17.7333ZM15.2 7.88467L12.559 10.5257C12.084 11.0007 11.4443 11.2667 10.7667 11.2667C9.348 11.2667 8.23333 10.152 8.23333 8.73333C8.23333 8.05567 8.49933 7.42233 8.97433 6.941L11.6153 4.3L10.8743 3.559C10.716 3.39433 10.5957 3.217 10.488 3.03333H1.26667C0.563667 3.03333 0 3.597 0 4.3V18.2333C0 18.93 0.563667 19.5 1.26667 19.5H15.2C15.8967 19.5 16.4667 18.93 16.4667 18.2333V9.01833C16.2893 8.917 16.1183 8.79667 15.9663 8.651L15.2 7.88467Z" fill="#037F72"></path>
                            </g>
                            <defs>
                                <clipPath id="clip0_1051_6167">
                                    <rect width="19" height="19" fill="white" transform="translate(0 0.5)"></rect>
                                </clipPath>
                            </defs>
                        </svg>
                    </a>
                </li>
            </ul>
        </template>
    </div>
</template>

<script setup>
    const props = defineProps({
        id: { type: [Number, String] }
    });

    const container = ref();
    const files = ref([]);
    const form = ref({});
    const images = ref([]);
    const videos = ref([]);

    const download = api.download;

    onMounted(() => {
        api.system("getNews", { ID: props.id }).subscribe((res) => {
            form.value = res.News;
            files.value = res.Files;
            images.value = res.Images;
            videos.value = res.Videos;

            nextTick(() => {
                Fancybox.bind(container.value, "[data-fancybox]");
            });
        });
    });
</script>
