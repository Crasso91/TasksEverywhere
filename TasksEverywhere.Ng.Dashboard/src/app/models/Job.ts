import { Call } from './Call';
export interface Job {
  JobID: number,
  ClientID: number,
  Key: string,
  Executing: string,
  StartedAt: Date,
  LastCall: Call,
  IsExecutingCorrectly: string,
  Description: string
}
