import { Error } from './Error'
export interface Call {
  callId: number,
  fireInstanceId: number,
  startedAt: Date,
  endedAt: Date,
  nextStart: Date,
  Error: Error
}
