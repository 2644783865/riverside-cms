var forms = (function () {
    return {
        allValidationData: [],
        errorMessages: function () {
            return [];
        },
        validateForm: function (validationData) {
            // Remove from all validation data any old messages for the current control that is being validated
            this.allValidationData = this.allValidationData.filter(function(item) {
                return item.id !== validationData.id;
            });
            // Register any new validation error messages
            if (validationData.errorMessages.length > 0)
                this.allValidationData.push(validationData);
            // Return whether or not form is valid
            return this.allValidationData.length === 0;
        },
        createEmptyCustomErrorMessages: function() {
            return { dateTime: new Date(), errors: [] };
        },
        createCustomErrorMessages: function(errors) {
            return { dateTime: new Date(), errors: errors };
        },
        getFilteredCustomErrorMessages: function (customErrorMessages, key) {
            return {
                dateTime: customErrorMessages.dateTime,
                errors: customErrorMessages.errors.filter(i => i.key === key).map(i => i.message)
            };
        }
    };
})();

Vue.component('flib-validation-summary', {
    props: {
        customErrorMessages: Function
    },
    computed: {
        errorMessages() {
            return this.customErrorMessages('').errors;
        }
    },
    template: `
        <div class="flib-validation-summary" :class="{ 'flib-error': errorMessages.length > 0 }" v-if="errorMessages.length > 0">
            <div role="alert" class="flib-messages">
                <ul>
                    <li v-for="message in errorMessages">{{message}}</li>
                </ul>
            </div>
        </div>
    `
});

Vue.component('flib-text-field', {
    props: {
        definition: {
            type: Object
        },
        index: {
            type: Number,
            default: null
        },
        state: {
            type: Object
        },
        value: {
            type: String,
            default: ''
        },
        readOnly: {
            type: Boolean,
            default: false
        },
        customErrorMessages: {
            type: Function,
            default: _value => forms.createEmptyCustomErrorMessages()
        },
        password: {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            selected: this.value,
            valueDateTime: null
        };
    },
    computed: {
        id() {
            return this.definition.id + (this.index === null ? '' : this.index);
        },
        errorMessages() {
            var messages = [];
            var customErrorMessages = this.customErrorMessages(this.definition.id);
            var displayCustomErrorMessages = this.valueDateTime === null || this.valueDateTime < customErrorMessages.dateTime;
            if (displayCustomErrorMessages)
                messages = messages.concat(this.customErrorMessages(this.definition.id).errors);
            var localValue = this.value === null ? '' : this.value;
            var trimmedValue = localValue.trim();
            var trimmedLength = trimmedValue.length;
            if (this.definition.required && trimmedLength === 0)
                messages.push(this.definition.requiredMessage);
            if (trimmedLength !== 0 && this.definition.minLength > 0 && trimmedLength < this.definition.minLength)
                messages.push(this.definition.lengthMessage);
            if (trimmedLength !== 0 && this.definition.maxLength > 0 && trimmedLength > this.definition.maxLength)
                messages.push(this.definition.lengthMessage);
            this.$emit('validate', { id: this.id, errorMessages: messages });
            return messages;
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': errorMessages.length > 0 }">
            <label :for="id">{{definition.label}}</label>
            <div class="flib-field-control">
                <input v-if="!definition.rows"
                    :type="password ? 'password' : 'text'"
                    :id="id"
                    :name="id"
                    :readonly="readOnly" 
                    autocomplete="off"
                    autocorrect="off"
                    autocapitalize="off"
                    spellcheck="false"
                    class="flib-input"
                    v-model="selected"
                    @input="updateValue"
                />
                <textarea v-if="definition.rows"
                    :id="id"
                    :name="id"
                    :readonly="readOnly"
                    autocomplete="off"
                    autocorrect="off"
                    autocapitalize="off"
                    spellcheck="false"
                    class="flib-input"
                    :rows="definition.rows"
                    v-model="selected"
                    @input="updateValue"
                />
                <small v-if="definition.helpMessage && !readOnly">{{definition.helpMessage}}</small>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueDateTime != null">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `,
    methods: {
        updateValue() {
            const newValue = this.selected; 
            this.valueDateTime = new Date();
            this.$emit('input', newValue);
        }
    }
});

Vue.component('flib-integer-field', {
    props: {
        definition: {
            type: Object
        },
        value: {
            type: [Number, String],
            required: false
        },
        readOnly: {
            type: Boolean,
            default: false
        },
        customErrorMessages: {
            type: Function,
            default: _value => forms.createEmptyCustomErrorMessages()
        }
    },
    data() {
        return {
            integer: `${this.value ? this.value : ``}`,
            valueDateTime: null
        };
    },
    computed: {
        id() {
            return this.definition.id + (this.index === null ? '' : this.index);
        },
        integerString() {
            return this.integer + '';
        },
        errorMessages() {
            var messages = [];
            let id = this.definition.id + (this.index === null ? '' : this.index);
            var customErrorMessages = this.customErrorMessages(id);
            var displayCustomErrorMessages = this.valueDateTime === null || this.valueDateTime < customErrorMessages.dateTime;
            if (displayCustomErrorMessages)
                messages = messages.concat(this.customErrorMessages(id).errors);
            var trimmedValue = this.integer.trim();
            var trimmedLength = trimmedValue.length;
            if (this.definition.required && trimmedLength === 0)
                messages.push(this.definition.requiredMessage);
            var integerValue = parseInt(trimmedValue);
            var invalid = trimmedLength !== 0 && (Number.isNaN(integerValue) || (integerValue + '').length !== trimmedValue.length);
            if (invalid)
                messages.push(this.definition.invalidMessage);
            if (!invalid && (this.definition.min !== null && this.definition.min !== undefined && integerValue < this.definition.min || this.definition.max !== null && this.definition.max !== undefined && integerValue > this.definition.max))
                messages.push(this.minMaxMessage);
            this.$emit('validate', { id: id, errorMessages: messages });
            return messages;
        }
    },
    methods: {
        updateValue() {
            var value = parseInt(this.integer);
            if (Number.isNaN(value))
                value = null;
            this.valueDateTime = new Date();
            this.$emit(`input`, value);
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': errorMessages.length > 0 }">
            <label :for="id">{{definition.label}}</label>
            <div class="flib-field-control">
                <input v-if="readOnly" class="flib-input" :id="id" :value="integerString" readonly />
                <input v-if="!readOnly" class="flib-input" :id="id" v-model="integer" type="number" @input="updateValue" />
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueDateTime != null">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `
});

Vue.component('flib-time-field', {
    props: {
        label: {
            type: String,
            default: 'Field label'
        },
        id: {
            type: String
        },
        value: {
            type: [String],
            required: false
        },
        required: {
            type: Boolean,
            default: false
        },
        requiredMessage: {
            type: String
        },
        invalidMessage: {
            type: String,
            default: 'Value is not a time in the format HH:MM:SS'
        },
        readOnly: {
            type: Boolean,
            default: false
        },
        customErrorMessages: {
            type: Function,
            default: _value => forms.createEmptyCustomErrorMessages()
        }
    },
    data() {
        return {
            time: `${this.value ? this.value : ``}`,
            valueDateTime: null
        };
    },
    computed: {
        timeString() {
            return this.time;
        },
        errorMessages() {
            var messages = [];
            var customErrorMessages = this.customErrorMessages(this.id);
            var displayCustomErrorMessages = this.valueDateTime === null || this.valueDateTime < customErrorMessages.dateTime;
            if (displayCustomErrorMessages)
                messages = messages.concat(this.customErrorMessages(this.id).errors);
            var trimmedValue = this.time.trim();
            var trimmedLength = trimmedValue.length;
            if (this.required && trimmedLength === 0)
                messages.push(this.requiredMessage);
            var regex = /(?:[01]\d|2[0123]):(?:[012345]\d):(?:[012345]\d)/;
            var invalid = trimmedLength !== 0 && !regex.test(trimmedValue);
            if (invalid)
                messages.push(this.invalidMessage);
            this.$emit('validate', { id: this.id, errorMessages: messages });
            return messages;
        }
    },
    methods: {
        updateValue() {
            var value = this.time;
            var regex = /(?:[01]\d|2[0123]):(?:[012345]\d):(?:[012345]\d)/;
            if (!regex.test(value))
                value = null;
            this.valueDateTime = new Date();
            this.$emit(`input`, value);
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': errorMessages.length > 0 }">
            <label :for="id">{{label}}</label>
            <div class="flib-field-control">
                <input v-if="readOnly" class="flib-input" :id="id" :value="timeString" readonly />
                <input v-if="!readOnly" class="flib-input" :id="id" v-model="time" type="text" @input="updateValue" placeholder="HH:MM:SS" />
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueDateTime != null">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `
});

Vue.component('flib-boolean-field', {
    model: {
        event: 'change'
    },
    props: {
        definition: {
            type: Object
        },
        value: {
            type: [Boolean, String],
            required: false
        },
        readOnly: {
            type: Boolean,
            default: false
        },
        customErrorMessages: {
            type: Function,
            default: _value => forms.createEmptyCustomErrorMessages()
        }
    },
    data() {
        return {
            boolean: this.value,
            valueDateTime: null
        };
    },
    computed: {
        id() {
            return this.definition.id + (this.index === null ? '' : this.index);
        },
        booleanString() {
            return this.boolean ? 'Yes' : 'No';
        },
        errorMessages() {
            var messages = [];
            let id = this.definition.id + (this.index === null ? '' : this.index);
            var customErrorMessages = this.customErrorMessages(id);
            var displayCustomErrorMessages = this.valueDateTime === null || this.valueDateTime < customErrorMessages.dateTime;
            if (displayCustomErrorMessages)
                messages = messages.concat(this.customErrorMessages(id).errors);
            this.$emit('validate', { id: id, errorMessages: messages });
            return messages;
        }
    },
    methods: {
        updateValue() {
            var value = this.boolean;
            this.valueDateTime = new Date();
            this.$emit(`change`, value);
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': errorMessages.length > 0 }">
            <label :for="id">{{definition.label}}</label>
            <div class="flib-field-control">
                <input v-if="readOnly" class="flib-input" :id="id" :value="booleanString" readonly />
                <div v-if="!readOnly" class="flib-checkbox"> 
                    <input type="checkbox" :id="id" v-model="boolean" @change="updateValue" />
                </div>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueDateTime != null">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `
});

Vue.component('flib-date-field', {
    props: {
        definition: {
            type: Object
        },
        value: {
            type: [Number, String],
            required: true
        },
        showDay: {
            type: Boolean,
            default: true
        },
        showMonth: {
            type: Boolean,
            default: true
        },
        showYear: {
            type: Boolean,
            default: true
        },
        readOnly: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            day: `${this.value ? new Date(this.value).getDate() : ``}`,
            month: `${this.value ? new Date(this.value).getMonth() : ``}`,
            year: `${this.value ? new Date(this.value).getFullYear() : ``}`,
            hours: `${this.value ? new Date(this.value).getHours() : ``}`,
            minutes: `${this.value ? new Date(this.value).getMinutes() : ``}`,
            seconds: `${this.value ? new Date(this.value).getSeconds() : ``}`
        };
    },
    watch: {
        year(current, prev) {
            if (current > 9999) this.year = prev;
        }
    },
    methods: {
        updateDay() {
            if (!this.day.length || parseInt(this.day, 10) < 4) return;
            if (this.showMonth) this.$refs.month.select();
            else if (this.showYear) this.$refs.year.select();
        },
        updateMonth() {
            if (!this.month.length || parseInt(this.month, 10) < 2) return;
            if (this.showYear) this.$refs.year.select();
        },
        updateValue() {
            const timestamp = Date.parse(`${this.year.padStart(4, 0)}-${this.month}-${this.day}`);
            if (Number.isNaN(timestamp)) return;
            this.$emit(`input`, timestamp);
        }
    },
    computed: {
        dateString() {
            var d = new Date(this.year, this.month, this.day, this.hours, this.minutes, this.seconds);
            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var years = d.getFullYear().toString().padStart(4, 0);
            var days = d.getDate().toString().padStart(2, 0);
            var hours = d.getHours().toString().padStart(2, 0);
            var mins = d.getMinutes().toString().padStart(2, 0);
            var seconds = d.getSeconds().toString().padStart(2, 0);
            return days + ' ' + months[d.getMonth()] + ' ' + years + ' ' + hours + ':' + mins + ':' + seconds + ' GMT';
        }
    },
    template: `
        <div class="flib-field">
            <label :for="definition.id">{{definition.label}}</label>
            <div class="flib-field-control">
                <div v-if="readOnly">
                    <input class="flib-input" :id="definition.id" :value="dateString" readonly />
                </div>
                <div @keyup.capture="updateValue" v-if="!readOnly">
                    <input
                      v-if="showDay"
                      ref="day"
                      v-model="day"
                      type="number"
                      placeholder="dd"
                      @input="updateDay"
                      @blur="day = day.padStart(2, 0)">
                    <span
                      v-if="showDay && showMonth"
                    >/</span>
                    <input
                      v-if="showMonth"
                      ref="month"
                      v-model="month"
                      type="number"
                      placeholder="mm"
                      @input="updateMonth"
                      @blur="month = month.padStart(2, 0)">
                    <span
                      v-if="showYear && (showDay || showMonth)"
                    >/</span>
                    <input
                      v-if="showYear"
                      ref="year"
                      v-model="year"
                      type="number"
                      placeholder="yyyy"
                      @blur="year = year.padStart(4, 0)">
                </div>
            </div>
        </div>
    `
});

Vue.component('flib-multi-select-field', {
    model: {
        event: 'change'
    },
    props: {
        definition: {
            type: Object
        },
        index: {
            type: Number,
            default: null
        },
        optionAdapter: {
            type: Function,
            default: value => ({
                id: value.id,
                label: value.name,
                value
            })
        },
        options: {
            type: Array,
            default: () => []
        },
        value: {
            type: [Array, String, Number, Object],
            default: null
        },
        readOnly: {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            selected: this.value,
            showSelected: false,
            valueChanged: false,
            editing: false,
            focussed: false,
            keepFocussed: false
        };
    },
    computed: {
        id() {
            return this.definition.id + (this.index === null ? '' : this.index);
        },
        adaptedOptions() {
            return this.options.map(x => this.optionAdapter(x));
        },
        selectedOptions() {
            if (!Array.isArray(this.value))
                return this.options.map(x => this.optionAdapter(x)).filter(item => item.id === this.value);
            else
                return this.options.map(x => this.optionAdapter(x)).filter(item => this.value.includes(item.id));
        },
        multiple() {
            return Array.isArray(this.value);
        },
        errorMessages() {
            let messages = [];
            if (this.definition.required) {
                if (this.multiple && this.value.length === 0) {
                    messages.push(this.definition.requiredMessage);
                } else if (!this.multiple && this.value === null) {
                    messages.push(this.definition.requiredMessage);
                }
            }
            let id = this.definition.id + (this.index === null ? '' : this.index);
            this.$emit('validate', { id: id, errorMessages: messages });
            return messages;
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': readOnly !== true && errorMessages.length > 0 }">
            <label :for="id">{{definition.label}}</label>
            <div class="flib-field-control" v-if="(readOnly !== true) && editing" tabindex="0" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" ref="control">
                <div class="flib-type-ahead" :class="{ 'flib-focussed': focussed }">
                    <div class="flib-type-ahead-panel">
                        <div class="flib-type-ahead-pannel-inner">
                            <div class="flib-type-ahead-panel-info flib-selection">
                                <span v-if="multiple"><a href="#" v-on:click.prevent="selectAll" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Select all</a>|<a href="#" v-on:click.prevent="clearAll" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Clear all</a></span>
                            </div>
                            <div class="flib-type-ahead-panel-actions flib-filter">
                                <a href="#" v-on:click.prevent="selectShowAll" :class="{ 'active': !showSelected }" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Full list</a><a href="#" v-on:click.prevent="selectShowSelected" :class="{ 'active': showSelected }" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Selected</a>
                            </div>
                        </div>
                    </div>
                    <div class="flib-type-ahead-options" :class="{ 'flib-multiple': multiple }">
                        <label v-if="!multiple && definition.nullOptionLabel">
                            <input :value="null" v-model="selected" type="radio" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">
                            {{definition.nullOptionLabel}}
                        </label>
                        <label v-for="option in adaptedOptions" :key="option.id" v-if="!showSelected || (showSelected && ((multiple && value.includes(option.id)) || (!multiple && (value === option.id))))"> 
                            <input :value="option.id" v-model="selected" type="checkbox" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" v-if="multiple">
                            <input :value="option.id" v-model="selected" type="radio" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" v-if="!multiple">
                            {{option.label}}
                        </label>
                    </div>
                    <div class="flib-type-ahead-panel flib-bottom">
                        <div class="flib-type-ahead-pannel-inner">
                            <div class="flib-type-ahead-panel-info">
                                {{selectedOptions.length}} selected
                            </div>
                            <div class="flib-type-ahead-panel-actions">
                                <a href="#" v-on:click.prevent="stopEditing" class="flib-action" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Close</a>
                            </div>
                        </div>
                    </div>                    
                </div>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueChanged">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
            <div class="flib-field-control" v-if="readOnly === true || (readOnly !== true && !editing)" tabindex="0" v-on:focus="onViewingFocus"  v-on:blur="onViewingBlur">
                <ul :id="id" class="flib-input" :class="{ 'flib-focussed': focussed, 'flib-read-only': readOnly }">
                    <li v-if="selectedOptions.length === 0" v-text="definition.emptyOptionLabel" />
                    <li v-for="option in selectedOptions" v-text="option.label" />
                </ul>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueChanged">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>

    `,
    watch: {
        focussed() {
            this.editing = this.focussed;
        }
    },
    methods: {
        selectAll() {
            this.selected = this.options.map(x => this.optionAdapter(x)).map(o => o.id);
            this.updateValue();
        },
        clearAll() {
            this.selected = [];
            this.updateValue();
        },
        selectShowAll() {
            this.showSelected = false;
        },
        selectShowSelected() {
            this.showSelected = true;
        },
        stopEditing() {
            this.editing = false;
        },
        onViewingFocus() {
            this.editing = true;
            this.focussed = true;
        },
        onEditingFocus() {
            this.focussed = true;
        },
        onViewingBlur() {
            this.focussed = false;
        },
        onEditingBlur: function() {
            if (!this.keepFocussed)
                this.focussed = false;
            else
                this.$refs.control.focus();
            this.keepFocussed = false;
        },
        updateValue() {
            if (this.multiple && this.showSelected)
                this.keepFocussed = true;
            const newValue = this.selected;
            this.valueChanged = true;
            this.$emit('change', newValue);
        }
    }
});

Vue.component('flib-list-field', {
    model: {
        event: 'change'
    },
    props: {
        emptyOption: {
            type: String,
            default: 'No items'
        },
        optionAdapter: {
            type: Function,
            default: value => ({
                id: value,
                label: value,
                value
            })
        },
        label: {
            type: String,
            default: 'Field label'
        },
        id: {
            type: String
        },
        options: {
            type: Array,
            default: () => []
        },
        value: {
            type: [Array, String, Number, Object],
            default: null
        }
    },
    computed: {
        selectedOptions() {
            if (!Array.isArray(this.value))
                return this.options.map(x => this.optionAdapter(x)).filter(item => item.id === this.value);
            else
                return this.options.map(x => this.optionAdapter(x)).filter(item => this.value.includes(item.id));
        }
    },
    template: `
        <div class="flib-field">
            <label :for="id">{{label}}</label>
            <div class="flib-field-control">
                <ul :id="id" class="flib-input" tabindex="0">
                    <li v-if="selectedOptions.length === 0" v-text="emptyOption" />
                    <li v-for="option in selectedOptions" v-text="option.label" />
                </ul>
            </div>
        </div>
    `
});

Vue.component('flib-button', {
    props: {
        definition: Object,
        state: Object,
        enabled: Boolean
    },
    data: function () {
        return {
            localDefinition: this.definition,
            localState: this.state
        };
    },
    template: `
        <button
            :id="definition.id"
            :name="definition.id"
            class="flib-button"
            :disabled="!enabled"
            type="submit"
            v-on:click.prevent="onButtonClick">{{definition.label}}</button>
    `,
    methods: {
        onButtonClick: function () {
            this.$emit('click', this.$data.localDefinition);
        }
    }
});

Vue.component('flib-option-text', {
    props: {
        emptyOption: {
            type: String,
            default: '-'
        },
        optionAdapter: {
            type: Function,
            default: value => ({
                id: value.id,
                label: value.name,
                value
            })
        },
        options: {
            type: Array,
            default: () => []
        },
        value: {
            type: [Array, String, Number, Object],
            default: null
        }
    },
    computed: {
        adaptedOptions() {
            return this.options.map(x => this.optionAdapter(x));
        },
        selectedOptions() {
            if (!Array.isArray(this.value))
                return this.options.map(x => this.optionAdapter(x)).filter(item => item.id === this.value).map(item => item.label);
            else
                return this.options.map(x => this.optionAdapter(x)).filter(item => this.value.includes(item.id)).map(item => item.label);
        }
    },
    template: `
        <span class="flib-option-text"><template v-if="selectedOptions.length === 0">{{emptyOption}}</template><template v-if="selectedOptions.length !== 0">{{selectedOptions.join(', ')}}</template></span>
    `
});

Vue.component('flib-text', {
    props: {
        emptyOption: {
            type: String,
            default: '-'
        },
        renderAsList: {
            type: Boolean,
            default: false
        },
        value: {
            type: [Array, String],
            default: null
        }
    },
    computed: {
        multiple() {
            return Array.isArray(this.value);
        }
    },
    template: `
        <span class="flib-text"><template v-if="value === null || value === '' || value.length === 0">{{emptyOption}}</template><template v-if="value !== null && value !== '' && !multiple">{{value}}</template><template v-if="value !== null && value !== '' && multiple && !renderAsList">{{value.join(', ')}}</template><template v-if="value !== null && value !== '' && multiple && renderAsList"><ul><li v-for="val in value">{{val}}</li></ul></template></span>
    `
});

Vue.component('flib-integer-text', {
    props: {
        emptyOption: {
            type: String,
            default: '-'
        },
        value: {
            type: [Array, Number],
            default: null
        }
    },
    computed: {
        multiple() {
            return Array.isArray(this.value);
        }
    },
    template: `
        <span class="flib-integer-text"><template v-if="value === null">{{emptyOption}}</template><template v-if="value !== null && !multiple">{{value}}</template><template v-if="value !== null && multiple">{{value.join(', ')}}</template></span>
    `
});

Vue.component('flib-boolean-text', {
    props: {
        emptyOption: {
            type: String,
            default: '-'
        },
        value: {
            type: [Boolean],
            default: null
        }
    },
    template: `
        <span class="flib-boolean-text"><template v-if="value === null">{{emptyOption}}</template><template v-if="value === true">Yes</template><template v-if="value === false">No</template></span>
    `
});

Vue.component('flib-date', {
    props: {
        emptyOption: {
            type: String,
            default: '-'
        },
        value: {
            type: [Number, String]
        }
    },
    data() {
        return {
            day: `${this.value ? new Date(this.value).getDate() : ``}`,
            month: `${this.value ? new Date(this.value).getMonth() : ``}`,
            year: `${this.value ? new Date(this.value).getFullYear() : ``}`,
            hours: `${this.value ? new Date(this.value).getHours() : ``}`,
            minutes: `${this.value ? new Date(this.value).getMinutes() : ``}`,
            seconds: `${this.value ? new Date(this.value).getSeconds() : ``}`
        };
    },
    computed: {
        dateString() {
            var d = new Date(this.year, this.month, this.day, this.hours, this.minutes, this.seconds);
            var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
            var years = d.getFullYear().toString().padStart(4, 0);
            var days = d.getDate().toString().padStart(2, 0);
            var hours = d.getHours().toString().padStart(2, 0);
            var mins = d.getMinutes().toString().padStart(2, 0);
            var seconds = d.getSeconds().toString().padStart(2, 0);
            return days + ' ' + months[d.getMonth()] + ' ' + years + ' ' + hours + ':' + mins + ':' + seconds + ' GMT';
        }
    },
    template: `
        <span class="flib-date"><template v-if="value === null || value === ''">{{emptyOption}}</template><template v-if="value !== null && value !== ''">{{dateString}}</template></span>
    `
});

Vue.component('flib-dynamic-select-field', {
    model: {
        event: 'change'
    },
    props: {
        definition: {
            type: Object
        },
        index: {
            type: Number,
            default: null
        },
        optionProvider: {
            type: Function
        },
        optionAdapter: {
            type: Function,
            default: value => ({
                id: value.id,
                label: value.name,
                value
            })
        },
        value: {
            type: [Array, String, Number, Object],
            default: null
        },
        readOnly: {
            type: Boolean,
            default: false
        }
    },
    data: function () {
        return {
            selected: this.value,
            valueChanged: false,
            editing: false,
            focussed: false,
            pageIndex: 0,
            pageSize: 50,
            searchText: '',
            options: []
        };
    },
    computed: {
        id() {
            return this.definition.id + (this.index === null ? '' : this.index);
        },
        adaptedOptions() {
            return this.options.map(x => this.optionAdapter(x));
        },
        selectedOptions() {
            if (!Array.isArray(this.value))
                return this.options.map(x => this.optionAdapter(x)).filter(item => item.id === this.value);
            else
                return this.options.map(x => this.optionAdapter(x)).filter(item => this.value.includes(item.id));
        },
        selectedOptionsCount() {
            if (!Array.isArray(this.value))
                return this.value !== null ? 1 : 0;
            else
                return this.value !== null ? this.value.length : 0;
        },
        multiple() {
            return Array.isArray(this.value);
        },
        errorMessages() {
            let messages = [];
            if (this.definition.required) {
                if (this.multiple && this.value.length === 0) {
                    messages.push(this.definition.requiredMessage);
                } else if (!this.multiple && this.value === null) {
                    messages.push(this.definition.requiredMessage);
                }
            }
            let id = this.definition.id + (this.index === null ? '' : this.index);
            this.$emit('validate', { id: id, errorMessages: messages });
            return messages;
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': readOnly !== true && errorMessages.length > 0 }">
            <label :for="id">{{definition.label}}</label>
            <div class="flib-field-control" v-if="(readOnly !== true) && editing" tabindex="0" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" ref="control">
                <div class="flib-type-ahead" :class="{ 'flib-focussed': focussed }">
                    <div class="flib-search-panel">
                        <input v-model="searchText" type="text" @input="updateOptions" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" class="flib-input" placeholder="Search" />
                    </div>
                    <div class="flib-type-ahead-options" :class="{ 'flib-multiple': multiple }">
                        <label v-if="!multiple && definition.nullOptionLabel">
                            <input :value="null" v-model="selected" type="radio" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">
                            {{definition.nullOptionLabel}}
                        </label>
                        <label v-for="option in options" :key="option.id">
                            <input :value="option.id" v-model="selected" type="checkbox" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" v-if="multiple">
                            <input :value="option.id" v-model="selected" type="radio" @change="updateValue" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur" v-if="!multiple">
                            {{option.label}}
                        </label>
                    </div>
                    <div class="flib-type-ahead-panel flib-bottom">
                        <div class="flib-type-ahead-pannel-inner">
                            <div class="flib-type-ahead-panel-info">
                                {{selectedOptionsCount}} selected
                            </div>
                            <div class="flib-type-ahead-panel-actions">
                                <a href="#" v-on:click.prevent="stopEditing" class="flib-action" v-on:focus="onEditingFocus" v-on:blur="onEditingBlur">Close</a>
                            </div>
                        </div>
                    </div>                    
                </div>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueChanged">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
            <div class="flib-field-control" v-if="readOnly === true || (readOnly !== true && !editing)" tabindex="0" v-on:focus="onViewingFocus"  v-on:blur="onViewingBlur">
                <ul :id="id" class="flib-input" :class="{ 'flib-focussed': focussed, 'flib-read-only': readOnly }">
                    <li v-if="selectedOptionsCount === 0" v-text="definition.emptyOptionLabel" />
                    <li v-if="selectedOptionsCount !== 0 && multiple" v-for="id in selected" v-text="id" />
                    <li v-if="selectedOptionsCount !== 0 && !multiple" v-text="selected" />
                </ul>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueChanged">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `,
    watch: {
        focussed() {
            this.editing = this.focussed;
        }
    },
    methods: {
        stopEditing() {
            this.editing = false;
        },
        onViewingFocus() {
            this.editing = true;
            this.focussed = true;
            this.updateOptions();
        },
        onEditingFocus() {
            this.focussed = true;
        },
        onViewingBlur() {
            this.focussed = false;
        },
        onEditingBlur: function() {
            this.focussed = false;
        },
        updateValue() {
            const newValue = this.selected;
            this.valueChanged = true;
            this.$emit('change', newValue);
        },
        updateOptions() {
            var vue = this;
            this.optionProvider(this.pageIndex, this.pageSize, this.searchText).then(function (items) {
                vue.options = items.map(i => vue.optionAdapter(i));
            });
        }
    }
});

Vue.component('flib-upload-field', {
    model: {
        event: 'change'
    },
    props: {
        definition: {
            type: Object
        },
        value: {
            type: [Array, String, Number, Object],
            default: null
        },
        readOnly: {
            type: Boolean,
            default: false
        },
        customErrorMessages: {
            type: Function,
            default: _value => forms.createEmptyCustomErrorMessages()
        }
    },
    data: function () {
        return {
            selected: this.value,
            valueChanged: false,
            storageResults: [],
            percentComplete: 0,
            uploading: false
        };
    },
    computed: {
        multiple() {
            return Array.isArray(this.value);
        },
        errorMessages() {
            var messages = [];
            if (this.definition.required) {
                if (this.multiple && this.value.length === 0) {
                    messages.push(this.definition.requiredMessage);
                } else if (!this.multiple && this.value === null) {
                    messages.push(this.definition.requiredMessage);
                }
            }
            this.$emit('validate', { id: this.definition.id, errorMessages: messages });
            return messages;
        }
    },
    template: `
        <div class="flib-field" :class="{ 'flib-error': readOnly !== true && errorMessages.length > 0 }">
            <label :for="definition.id">{{definition.label}}</label>
            <div class="flib-field-control">
                <label :for="definition.id" class="flib-input" v-if="!readOnly">Browse...</label>
                <input 
                    type="file"
                    :id="definition.id"
                    :name="definition.id"
                    :readonly="readOnly" 
                    autocomplete="off"
                    autocorrect="off"
                    autocapitalize="off"
                    spellcheck="false"
                    class="flib-input"
                    :multiple="multiple"
                    @change="updateValue($event.target.files)"
                    v-if="!readOnly"
                />
                <input type="text" :id="definition.id" :name="definition.id" :readonly="readOnly" class="flib-input" v-if="readOnly" />
                <div class="flib-progress" v-if="uploading">
                    <div class="flib-progress-percent" :style="{ width: percentComplete + '%' }">{{percentComplete}}%</div>
                </div>
                <div role="alert" class="flib-uploaded" v-if="storageResults.length > 0">
                    <ul>
                        <li v-for="result in storageResults">{{result.name}} ({{result.size}} bytes)</li>
                    </ul>
                </div>
                <div role="alert" class="flib-messages" v-if="errorMessages.length > 0 && valueChanged">
                    <ul>
                        <li v-for="message in errorMessages">{{message}}</li>
                    </ul>
                </div>
            </div>
        </div>
    `,
    methods: {
        uploadFiles(files) {
            var vue = this;
            var config = {
                onUploadProgress: function(progressEvent) {
                    vue.percentComplete = Math.round(progressEvent.loaded * 100 / progressEvent.total);
                },
                headers: {
                    'Content-Type': 'multipart/form-data',
                    'Authorization': 'Bearer ' + auth.userSession().security.token
                }
            };
            vue.uploading = true;
            vue.percentComplete = 0;
            axios.post(vue.definition.uploadUrl, files, config)
                .then(function (result) {
                    vue.uploading = false;
                    vue.percentComplete = 0;
                    var storageResults = result.data;
                    if (storageResults.length === 0)
                        vue.selected = vue.multiple ? [] : null;
                    else
                        vue.selected = vue.multiple ? storageResults.map(r => r.id) : storageResults[0].id;
                    vue.valueChanged = true;
                    vue.storageResults = storageResults;
                    vue.$emit('change', vue.selected);
                })
                .catch(function (err) {
                    vue.uploading = false;
                    vue.percentComplete = 0;
                    console.log(err.message);
                });
        },
        updateValue(fileList) {
            var files = new FormData();
            for (let i = 0; i < fileList.length; i++) {
                files.append("file", fileList[i], fileList[i].name);
            }
            this.selected = this.multiple ? [] : null;
            this.valueChanged = false;
            this.$emit('change', this.selected);
            this.uploadFiles(files);
        }
    }
});