import { useState } from "react"
import styles from "./weather.module.css"
import { useGetWeatherForecastsQuery } from "./weatherApiSlice"


export const Weather = () => {
  // Using a query hook automatically fetches data and returns query values
  const {
    data: forecasts,
    isError,
    isLoading,
    isSuccess,
  } = useGetWeatherForecastsQuery()

  if (isError) {
    return (
      <div>
        <h1>There was an error!!!</h1>
      </div>
    )
  }

  if (isLoading) {
    return (
      <div>
        <h1>Loading...</h1>
      </div>
    )
  }

  if (isSuccess) {
    return (
      <div>
        <h1>Weather Forecast</h1>
        <ul>
          {forecasts?.map(forecast => (
            <li key={forecast.date}>
              {forecast.date}: {forecast.temperatureC}°C, {forecast.summary}
            </li>
          ))}
        </ul>
      </div>
    )
  }

  return null
}
