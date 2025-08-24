let current = Date.now();

export const isProjectEditable = (type, status, step) => {
    if (status === 1) {
        return true;
    }

    return false;
};

export const nextId = () => `id--${current++}`;

export const toInt = (value) => parseInt(value) || 0;
