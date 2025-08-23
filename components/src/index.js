import AccessibilityApplication from "./components/Accessibility/Application.vue";
import AccessibilityAttachment from "./components/Accessibility/Attachment.vue";
import AccessibilityBenefit from "./components/Accessibility/Benefit.vue";
import AccessibilityFunding from "./components/Accessibility/Funding.vue";
import AccessibilityProgressBar from "./components/Accessibility/ProgressBar.vue";
import AccessibilityWorkSchedule from "./components/Accessibility/WorkSchedule.vue";
import CommonProgressBar from "./components/CommonProgressBar.vue";
import CultureApplication from "./components/Culture/Application.vue";
import CultureAttachment from "./components/Culture/Attachment.vue";
import CultureFunding from "./components/Culture/Funding.vue";
import CultureOther from "./components/Culture/Other.vue";
import CultureProgressBar from "./components/Culture/ProgressBar.vue";
import CultureWorkSchedule from "./components/Culture/WorkSchedule.vue";
import EducationApplication from "./components/Education/Application.vue";
import EducationAttachment from "./components/Education/Attachment.vue";
import EducationProgressBar from "./components/Education/ProgressBar.vue";
import ErrorModal from "./components/ErrorModal.vue";
import InputBoolean from "./components/InputBoolean.vue";
import InputFile from "./components/InputFile.vue";
import InputInteger from "./components/InputInteger.vue";
import InputMonth from "./components/InputMonth.vue";
import InputRadioGroup from "./components/InputRadioGroup.vue";
import InputSelect from "./components/InputSelect.vue";
import InputText from "./components/InputText.vue";
import InputTextarea from "./components/InputTextarea.vue";
import InputTwDate from "./components/InputTwDate.vue";
import LiteracyApplication from "./components/Literacy/Application.vue";
import LiteracyAttachment from "./components/Literacy/Attachment.vue";
import LiteracyBenefit from "./components/Literacy/Benefit.vue";
import LiteracyFunding from "./components/Literacy/Funding.vue";
import LiteracyProgressBar from "./components/Literacy/ProgressBar.vue";
import LiteracyWorkSchedule from "./components/Literacy/WorkSchedule.vue";
import MultipleApplication from "./components/Multiple/Application.vue";
import MultipleAttachment from "./components/Multiple/Attachment.vue";
import MultipleBenefit from "./components/Multiple/Benefit.vue";
import MultipleFunding from "./components/Multiple/Funding.vue";
import MultipleProgressBar from "./components/Multiple/ProgressBar.vue";
import MultipleWorkSchedule from "./components/Multiple/WorkSchedule.vue";
import RequiredLabel from "./components/RequiredLabel.vue";

const components = {
    AccessibilityApplication,
    AccessibilityAttachment,
    AccessibilityBenefit,
    AccessibilityFunding,
    AccessibilityProgressBar,
    AccessibilityWorkSchedule,
    CommonProgressBar,
    CultureApplication,
    CultureAttachment,
    CultureFunding,
    CultureOther,
    CultureProgressBar,
    CultureWorkSchedule,
    EducationApplication,
    EducationAttachment,
    EducationProgressBar,
    ErrorModal,
    InputBoolean,
    InputFile,
    InputInteger,
    InputMonth,
    InputRadioGroup,
    InputSelect,
    InputText,
    InputTextarea,
    InputTwDate,
    LiteracyApplication,
    LiteracyAttachment,
    LiteracyBenefit,
    LiteracyFunding,
    LiteracyProgressBar,
    LiteracyWorkSchedule,
    MultipleApplication,
    MultipleAttachment,
    MultipleBenefit,
    MultipleFunding,
    MultipleProgressBar,
    MultipleWorkSchedule,
    RequiredLabel
};

export default {
    install: (app) => {
        for (const name in components) {
            app.component(name, components[name]);
        }
    }
};
