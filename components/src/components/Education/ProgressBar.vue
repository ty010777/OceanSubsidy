<template>
    <common-progress-bar :steps="steps" type="education"></common-progress-bar>
</template>

<script setup>
    const steps = [
        { step: 1, title: "申請表／計畫書", url: "Application" },
        { step: 2, title: "上傳附件／提送申請", url: "Attachment" }
    ];
</script>
