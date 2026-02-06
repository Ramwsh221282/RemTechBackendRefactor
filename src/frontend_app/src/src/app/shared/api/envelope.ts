export type TypedEnvelope<T> = {
  statusCode: number,
  body?: T | null,
  error?: string | null
}

export type Envelope = {
  statusCode: number,
  error?: string | null
}
