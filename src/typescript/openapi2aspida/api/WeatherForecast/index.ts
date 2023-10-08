/* eslint-disable */
import type * as Types from '../@types'

export type Methods = {
  get: {
    status: 200
    /** Success */
    resBody: Types.WeatherForecast[]
  }

  post: {
    status: 200
    /** Success */
    resBody: Types.WeatherForecast
    reqBody: Types.WeatherForecast
  }
}
