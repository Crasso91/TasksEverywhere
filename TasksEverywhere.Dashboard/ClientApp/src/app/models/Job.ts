import { Call } from './Call';
export interface Job {
  jobID: number,
  clientID: number,
  Key: string,
  executing: string,
  startedAt: Date,
  LastCall: Call,
}
