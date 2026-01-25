export type UpdateParserLinksRequest = {
  Links: UpdateParserLinksRequestPayload[]
}

export type UpdateParserLinksRequestPayload = {
  LinkId: string,
  Activity?: boolean | null,
  Name?: string | null,
  Url?: string | null
}

export type AddLinksToParserRequest = {
  Links: AddLinksToParserRequestBody[]
}

export type AddLinksToParserRequestBody = {
  Name: string,
  Url: string
}
