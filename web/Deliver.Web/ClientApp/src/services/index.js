import axios, {AxiosRequestConfig, AxiosResponse} from 'axios';
import HttpError from "./HttpError";
import Constants from "../Constants";
import Auth from '../components/api-authorization/AuthorizeService'
//import store from "../../store";
//import GlobalHelpers from "../GlobalHelpers";

async function getAuthToken() {
  if(await Auth.isAuthenticated())
    return await Auth.getAccessToken();

  return null;
}

/**
 * Requests a URL, returning a promise.
 *
 * @param  {string} url       The URL we want to request
 * @param  {object} [options] The options we want to pass to "fetch"
 * @return {object}           An object containing either "data" or "err"
 */
async function request(url, options) {
  const defaultOptions = {
    baseURL: Constants.APIBASE,
    url,
    method: 'get',
    headers: {
    },
  };

  const newOptions = {...defaultOptions, ...options};

  if (url.startsWith('/')) {
    const accessToken = await getAuthToken();
    if (accessToken) {
      newOptions.headers.Authorization = `Bearer ${accessToken}`;
    }
  } else {
    // all our api urls should begin with /, we do not add access token to anything else
    // as we might leak the token by making a call to a different origin via request.js helper
    window.console.warn(`Access Token not added to URL ${url} as it does not begin with '/'`);
  }

  let response;

  try {
    //GlobalHelpers.startProgress();
    response = await axios.request(newOptions);

    return [response.data, response];

  } catch (e) {
    const error = new HttpError(e);

    if (error.code === 'UNAUTHORIZED') {
      window.location.href = "/authentication/logout";
    }

    throw error;
  } finally {
    //GlobalHelpers.endProgress();
  }
}

/**
 * Helper for POST request
 *
 * @param url
 * @param data
 */
async function post(url, data = {}) {
  const [result, _] = await request(url, {
    method: 'POST',
    data,
  });

  return result;
}

/**
 * Helper to make a PUT request
 *
 * @param url
 * @param data
 * @returns {Object}
 */
async function put(url, data = {}) {
  const [result, _] = await request(url, {
    method: 'PUT',
    data,
  });

  return result;
}

/**
 * Helper to make a PATCH request
 *
 * @param url
 * @param data
 * @returns {Object}
 */
async function patch(url, data = {}) {
  const [result, _] = await request(url, {
    method: 'PATCH',
    data,
  });

  return result;
}

/**
 * Helper to make a GET request
 *
 * @param url
 * @param params
 * @returns {Object}
 */
async function get(url, params = {}) {
  const [result, _] = await request(url, {
    method: 'GET',
    params,
  });

  return result;
}

/**
 * Get raw response
 *
 * @param url
 * @param params
 */
async function getRaw(url, params = {}) {
  const [_, raw] = await request(url, {
    method: 'GET',
    params,
  });

  return raw.data;
}

/**
 * Helper to make a DELETE request
 *
 * @param url
 * @param params
 * @returns {Object}
 */
async function destroy(url, params = {}) {
  const [result, _] = await request(url, {
    method: 'DELETE',
    params,
  });

  return result;
}

/**
 * Default exports
 */
const HttpService = {
  request,
  get,
  getRaw,
  post,
  put,
  patch,
  destroy,
};

export default HttpService;
