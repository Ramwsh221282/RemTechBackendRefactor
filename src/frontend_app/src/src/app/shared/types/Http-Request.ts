export class HttpRequestBuilder {
  public static create(
    urlString: string,
    method: 'POST' | 'PUT' | 'GET' | 'PATCH' | 'DELETE' | string
  ): CustomHttpRequestWrapper {
    return new CustomHttpRequestWrapper(urlString, method);
  }
}

class CustomHttpRequestWrapper {
  private readonly _httpUrl: CustomHttpUrl;
  private readonly _httpMethod: CustomHttpMethod;
  private _httpParameters: CustomHttpParam[] = [];
  private _httpHeaders: CustomHttpHeader[] = [];
  private _httpBody: object | null = null;
  private _abortSignal: AbortSignal | null = null;

  public constructor(
    urlString: string,
    method: 'POST' | 'PUT' | 'GET' | 'PATCH' | 'DELETE' | string
  ) {
    this._httpUrl = { url: urlString };
    this._httpMethod = { method: method };
  }

  public addParameter(name: string, value: string): CustomHttpRequestWrapper {
    const newRequest: CustomHttpRequestWrapper = this.clone();
    if (value === 'null') return newRequest;
    if (value.trim().length === 0) return newRequest;
    newRequest._httpParameters.push({ name: name, value: value });
    return newRequest;
  }

  public addHeader(name: string, value: string): CustomHttpRequestWrapper {
    const newRequest: CustomHttpRequestWrapper = this.clone();
    newRequest._httpHeaders.push({ name: name, value: value });
    return newRequest;
  }

  public addBody(body: object): CustomHttpRequestWrapper {
    const newRequest: CustomHttpRequestWrapper = this.clone();
    newRequest._httpBody = body;
    return newRequest;
  }

  public addAbortSignal(abortSignal: AbortSignal): CustomHttpRequestWrapper {
    const newRequest: CustomHttpRequestWrapper = this.clone();
    newRequest._abortSignal = abortSignal;
    return newRequest;
  }

  public createFetchFunction(): Promise<Response> {
    const url: URL = this.createUrl();
    const options: RequestInit = this.createOptions();
    return fetch(url.toString(), options);
  }

  private createUrl(): URL {
    const url: URL = new URL(this._httpUrl.url);
    this._httpParameters.forEach((param) =>
      url.searchParams.append(param.name, param.value)
    );
    return url;
  }

  private createOptions(): RequestInit {
    const headers: HeadersInit = new Headers();
    this._httpHeaders.forEach((header) =>
      headers.append(header.name, header.value)
    );
    const options: RequestInit = {
      method: this._httpMethod.method,
      headers: headers,
      signal: this._abortSignal ?? undefined,
    };
    if (this._httpBody) options.body = JSON.stringify(this._httpBody);
    return options;
  }

  private clone(): CustomHttpRequestWrapper {
    const clone = new CustomHttpRequestWrapper(
      this._httpUrl.url,
      this._httpMethod.method
    );
    clone._httpParameters = this._httpParameters;
    clone._httpHeaders = this._httpHeaders;
    clone._httpBody = this._httpBody;
    clone._abortSignal = this._abortSignal;
    return clone;
  }
}

type CustomHttpUrl = {
  url: string;
};

type CustomHttpMethod = {
  method: string;
};

type CustomHttpParam = {
  name: string;
  value: string;
};

type CustomHttpHeader = {
  name: string;
  value: string;
};

type CustomHttpRequestBody = {
  body: object;
};
