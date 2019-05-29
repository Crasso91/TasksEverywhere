import { Call } from '.';

export interface Last20CallResponse {
  Last10Calls: Call[];
  Last10ErrorsCalls: Call[];
}
