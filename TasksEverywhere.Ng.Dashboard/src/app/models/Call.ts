import { Error } from './Error';
import { Job } from '.';

export interface Call {
  callId: number;
  fireInstanceId: number;
  startedAt: Date;
  endedAt: Date;
  nextStart: Date;
  Error: Error;
  JobID: number;
  Job: Job;
}
