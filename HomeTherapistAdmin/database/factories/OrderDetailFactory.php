<?php

namespace Database\Factories;

use App\Models\Order;
use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\OrderDetail>
 */
class OrderDetailFactory extends Factory
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
        $services = config('seeding.services');
        $service = $services->random();

        return [
            'order_id' => $order['id'],
            'service_id' => $service['id'],
            'price' => $service['price'],
            'note' => $this->faker->sentence,
        ];

    }
}
