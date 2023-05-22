<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\AppointmentDetail>
 */
class AppointmentDetailFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $appointments = config('seeding.appointments');
        $services = config('seeding.services');
        $service = $services->random();
        $appointment = $this->faker->randomElement($appointments);

        return [
            'appointment_id' => $appointment['id'],
            'service_id' => $service['id'],
            'price' => $service['price'],
            'note' => $this->faker->sentence,
        ];

    }
}
