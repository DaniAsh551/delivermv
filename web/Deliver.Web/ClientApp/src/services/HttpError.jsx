import { AxiosError } from "axios";


const codeMessage = {
    200: 'Requests successful',
    201: 'Resource created',
    202: 'Requests has been queued for processing',
    204: 'Data deleted successfully',

    400: 'There was an error in the request. No modifications were done to the data',
    401: 'User is not authorized',
    403: 'User is authorized but does not have permission for the action',
    404: 'Record does not exist',
    406: 'Requested format not available',
    410: 'The requested resource is permanently deleted and will no longer be available',
    422: 'A validation error occurred while creating an object',
    500: 'here was an error in the server. Please retry or inform the administrator',
    502: 'The gateway is wrong',
    503: 'Service is unavailable, the server is temporarily overloaded or maintained',
    504: 'The gateway timed out',
};

class HttpError extends Error {

    /**
     * Error code (Eg: VALIDATION_ERROR)
     */
    code;

    /**
     * Error REF
     */
    ref;

    /**
     * Error Name
     */
    name;

    /**
     * Axios Error
     */
    error;

    /**
     * Error timestamp
     */
    ts;

    /**
     * Form errors object
     */
    formErrors;

    /**
     * Constructor
     *
     * @param e
     */
    constructor(e) {
        if (!e.response) {
            super(e.message);

            this.code = 'NO_RESPONSE';
            this.ref = 'NO-RES';
            this.name = 'No Response';
            this.formErrors = null;

        } else if (!e.response.data || !e.response.data.errorCode) {
            super(e.message);

            this.code = 'ERROR';
            this.ref = 'GEN-ERROR';
            this.name = 'Error';
            this.formErrors = null;

        }
        else
        {
            // valid error from API
            const data = e.response.data;

            super(data.message || codeMessage[e.response.status]);

            this.code = data.errorCode || `${e.response.statusText}`;
            this.ref = data.errorRef;
            this.name = data.errorCodeText || this.code;
            this.formErrors = null;

            if (this.code === 'VALIDATION_ERROR') {
                this.formErrors = this.buildFormError(data)
            }
        }

        this.ts = +new Date;
        this.error = e;

        console.error({
            message: `Error: ${this.code}`,
            description: this.message,
        });
    }

    buildFormError(data) {
        return data.data;
    }
}

export default HttpError;
