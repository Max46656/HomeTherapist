<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Order>
 */
class OrderFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $calendars = config('seeding.calendars');
        $start_dt = $this->faker->randomElement($calendars);
        $users = config('seeding.users');
        $userId = $this->faker->randomElement($users);

        return [
            'user_id' => $userId,
            'start_dt' => $start_dt,
            'customer_ID' => $this->faker->regexify('[A-Z][1-2][0-9]{8}'),
            'customer_phone' => $this->faker->numerify('09########'),
            'customer_address' => $this->faker->address,
            'latitude' => $this->faker->latitude(21.5, 25.5, 7),
            'longitude' => $this->faker->longitude(120, 122, 7),
            'gender' => $this->faker->randomElement(['男', '女', '其他']),
            'age_group' => $this->faker->randomElement(['小於18', '18到25', '26到35', '36到45', '46到55', '56到65', '66到75', '大於75']),
            'is_complete' => true,
        ];
    }
}
