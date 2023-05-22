<?php

namespace Database\Factories;

use App\Models\Service;
use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\TherapistOpenServices>
 */
class TherapistOpenServicesFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $users = config('seeding.users');
        $services = config('seeding.services');
        $service = $services->random();
        $userId = $this->faker->randomElement($users);

        return [
            'user_id' => $userId,
            'service_id' => $service['id'],
        ];
    }
}
