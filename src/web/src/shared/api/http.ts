import { ApiRequestError, type ApiError, type ApiErrorResponse } from './errors'

type RequestBody = BodyInit | Record<string, unknown> | null

type ApiRequestOptions = Omit<RequestInit, 'body' | 'credentials'> & {
  body?: RequestBody
}

export async function apiRequest<TResponse>(
  path: string,
  options: ApiRequestOptions = {},
): Promise<TResponse> {
  const response = await fetch(path, {
    ...options,
    credentials: 'include',
    headers: buildHeaders(options),
    body: serializeBody(options.body),
  })

  if (!response.ok) {
    throw new ApiRequestError(response.status, await readErrors(response))
  }

  if (response.status === 204) {
    return undefined as TResponse
  }

  return (await response.json()) as TResponse
}

function buildHeaders(options: ApiRequestOptions): Headers {
  const headers = new Headers(options.headers)

  if (isJsonBody(options.body) && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  return headers
}

function serializeBody(body: RequestBody | undefined): BodyInit | null | undefined {
  if (isJsonBody(body)) {
    return JSON.stringify(body)
  }

  return body
}

function isJsonBody(body: RequestBody | undefined): body is Record<string, unknown> {
  return body != null && !(body instanceof FormData) && !(body instanceof Blob)
}

async function readErrors(response: Response): Promise<ApiError[]> {
  const fallbackError: ApiError = {
    code: 'http.request.failed',
    message: `Request failed with status ${response.status}.`,
    type: 0,
  }

  try {
    const body = (await response.json()) as Partial<ApiErrorResponse>

    if (Array.isArray(body.errors) && body.errors.length > 0) {
      return body.errors
    }
  } catch {
    return [fallbackError]
  }

  return [fallbackError]
}
