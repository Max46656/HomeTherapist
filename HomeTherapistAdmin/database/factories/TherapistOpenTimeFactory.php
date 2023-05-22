<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;
use Illuminate\Support\Arr;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\TherapistOpenTime>
 */
class TherapistOpenTimeFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $users = config('seeding.users');
        $calendars = config('seeding.calendars');

        return [
            'user_id' => Arr::random($users),
            'start_dt' => Arr::random($calendars),
        ];
    }
}
