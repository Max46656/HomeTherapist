<?php

namespace Database\Factories;

use Carbon\Carbon;
use Illuminate\Database\Eloquent\Factories\Factory;
use Illuminate\Support\Facades\DB;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Appointment>
 */
class AppointmentFactory extends Factory
{
    public function definition(): array
    {
        $users = config('seeding.users');
        $calendars = config('seeding.calendars');
        $userId = $this->faker->randomElement($users);
        $start_dt = $this->faker->randomElement($calendars);
        $start_dt = Carbon::parse($start_dt);

        return [
            'user_id' => $userId,
            'start_dt' => $start_dt,
            'customer_ID' => $this->faker->regexify('[A-Z][1-2][0-9]{8}'),
            'customer_phone' => $this->faker->numerify('09########'),
            'customer_address' => $this->faker->address,
            'latitude' => $this->faker->latitude(21.5, 25.5, 7),
            'longitude' => $this->faker->longitude(120, 122, 7),
            'is_complete' => strtotime($start_dt->format('Y-m-d')) < strtotime('2023-06-06') ? true : false,
        ];
    }
}