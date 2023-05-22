<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class AppointmentDetail extends Model
{
    use CrudTrait;
    use HasFactory;

    public function appointment(): BelongsTo
    {
        return $this->belongsTo(Order::class, 'user_id', 'staff_ID');
    }
}
