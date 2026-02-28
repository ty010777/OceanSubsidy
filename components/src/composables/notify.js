const instances = {};

const confirm = (config) => {
    if (!instances.confirm) {
        instances.confirm = create({
            cancelButtonText: "取消",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "確認"
        });
    }

    return instances.confirm.fire(config);
};

const create = (config) => Swal.mixin({ confirmButtonText: "確認", theme: "bootstrap-5-light", ...config });

const error = (config) => invoke("error", config);

const invoke = (type, config) => {
    if (!instances[type]) {
        instances[type] = create({ icon: type });
    }

    return instances[type].fire(config);
};

const success = (config) => invoke("success", config);

const warning = (config) => invoke("warning", config);

export const notify = Object.assign(success, { confirm, error, success, warning });
