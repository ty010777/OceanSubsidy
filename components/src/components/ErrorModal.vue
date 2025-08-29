<template>
    <teleport to="body">
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="d-flex flex-column align-items-center mb-5 gap-3">
                            <h4 class="text-danger fw-bold">錯誤</h4>
                            <div class="fs-18">{{ message.error }}</div>
                        </div>
                        <div class="d-flex gap-4 flex-wrap justify-content-center">
                            <button class="btn btn-danger" data-bs-dismiss="modal" type="button">
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
    const message = useMessageStore();
    const modal = ref();

    onMounted(() => modal.value.addEventListener("hidden.bs.modal", () => message.error = null));

    watch(() => message.error, () => {
        if (message.error) {
            bootstrap.Modal.getOrCreateInstance(modal.value).show();
        }
    });
</script>
