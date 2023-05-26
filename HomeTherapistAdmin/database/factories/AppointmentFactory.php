<?php

namespace Database\Factories;

use Carbon\Carbon;
use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Appointment>
 */
class AppointmentFactory extends Factory
{
    public function definition(): array
    {
        $users = config('seeding.users');
        $calendars = config('seeding.calendars');
        $user_id = $this->faker->randomElement($users);
        // $targetDate = '2023-06-06';
        $start_dt = $this->faker->randomElement($calendars);
        $start_dt = Carbon::parse($start_dt);

        return [
            'user_id' => $user_id,
            'start_dt' => $start_dt,
            'customer_ID' => $this->faker->regexify('[A-Z][1-2][0-9]{8}'),
            'customer_phone' => $this->faker->numerify('09########'),
            'customer_address' => $this->faker->address,
            'latitude' => $this->faker->latitude(21.5, 25.5, 7),
            'longitude' => $this->faker->longitude(120, 122, 7),
            'gender' => $this->faker->randomElement(['其他', '男', '女'], [20, 40, 40]),
            'age_group' => $this->faker->randomElement(['小於18', '18-25', '26-35', '36-45', '46-55', '56-65', '66-75', '大於75']),
            'is_complete' => strtotime($start_dt->format('Y-m-d')) < strtotime('2023-06-06') ? true : false,
        ];
    }
}
