<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Feedback>
 */
class FeedbackFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $orders = config('seeding.orders');
        $order = $this->faker->randomElement($orders);
        $users = config('seeding.users');
        $userId = $this->faker->randomElement($users);

        return [
            'user_id' => $userId,
            'order_id' => $order['id'],
            'customer_id' => $order['customer_ID'],
            'comments' => $this->faker->realText(200),
            'rating' => $this->faker->numberBetween(1, 5),
        ];

    }
}
