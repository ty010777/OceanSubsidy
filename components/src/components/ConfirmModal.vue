<template>
    <teleport to="body">
        <div aria-hidden="true" class="modal fade" data-bs-backdrop="static" ref="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-md">
                <div class="modal-content">
                    <div class="modal-header">
                        <button aria-label="Close" class="btn-close" data-bs-dismiss="modal" type="button">
                            <i class="fa-solid fa-circle-xmark"></i>
                        </button>
                    </div>
                    <div class="modal-body">
                        <slot></slot>
                        <div class="d-flex justify-content-center mt-4 gap-4">
                            <button class="btn btn-gray" data-bs-dismiss="modal" type="button">{{ props.cancel }}</button>
                            <button class="btn btn-teal" @click="confirm" data-bs-dismiss="modal" type="button">{{ props.confirm }}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </teleport>
</template>

<script setup>
    let callback;

    const modal = ref();

    const props = defineProps({
        cancel: { default: "取消", type: String },
        confirm: { default: "確定", type: String }
    });

    const confirm = () => {
        if (callback) {
            callback();
        }
    };

    const show = (confirm) => {
        callback = confirm;

        bootstrap.Modal.getOrCreateInstance(modal.value).show();
    };

    defineExpose({ show });
</script>
