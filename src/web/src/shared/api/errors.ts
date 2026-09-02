export type ApiError = {
  code: string
  message: string
  type: number
}

export type ApiErrorResponse = {
  errors: ApiError[]
}

export class ApiRequestError extends Error {
  readonly status: number
  readonly errors: ApiError[]

  constructor(status: number, errors: ApiError[]) {
    super(errors[0]?.message ?? `Request failed with status ${status}`)
    this.name = 'ApiRequestError'
    this.status = status
    this.errors = errors
  }
}
