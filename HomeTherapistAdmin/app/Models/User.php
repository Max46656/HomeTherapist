<?php

namespace App\Models;

use Backpack\CRUD\app\Models\Traits\CrudTrait;
use Illuminate\Database\Eloquent\Factories\HasFactory;
// use Illuminate\Contracts\Auth\MustVerifyEmail;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Illuminate\Support\Str;
use Laravel\Sanctum\HasApiTokens;
use MatanYadaev\EloquentSpatial\Objects\Point;
use Spatie\Permission\Traits\HasRoles;

class User extends Authenticatable
{
    use HasRoles;
    use CrudTrait;
    use HasApiTokens, HasFactory, Notifiable;

    // protected $primaryKey = 'staff_id';
    protected $primaryKey = 'id';
    // public $incrementing = false;
    // protected $keyType = 'string';

    // protected static function boot()
    // {
    //     parent::boot();

    //     static::creating(function ($model) {
    //         $model->{$model->getKeyName()} = Str::uuid()->toString();
    //     });
    // }

    /**
     * The attributes that are mass assignable.
     *
     * @var array<int, string>
     */
    protected $fillable = [
        'username',
        'normalized_username',
        'email',
        'normalized_email',
        'email_confirmed',
        'password_hash',
        'security_stamp',
        'concurrency_stamp',
        'phone_number',
        'phone_number_confirmed',
        'two_factor_enabled',
        'lockout_end',
        'lockout_enabled',
        'access_failed_count',
        'staff_id',
        'address',
        'latitude',
        'longitude',
    ];

    /**
     * The attributes that should be hidden for serialization.
     *
     * @var array<int, string>
     */
    protected $hidden = [
        'password_hash',
        'security_stamp',
        'concurrency_stamp',
        'remember_token',
    ];

    /**
     * The attributes that should be cast.
     *
     * @var array<string, string>
     */
    protected $casts = [
        'email_confirmed' => 'boolean',
        'phone_number_confirmed' => 'boolean',
        'two_factor_enabled' => 'boolean',
        'lockout_enabled' => 'boolean',
        'access_failed_count' => 'integer',
        'lockout_end' => 'datetime',
    ];

    public function getStaffIdAttribute($value)
    {
        return $value;
        // return $this->staff_id;
    }
    /**
     * Get all of the article for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\HasMany
     */
    public function article(): HasMany
    {
        return $this->hasMany(Article::class, 'user_id', 'staff_id');
    }

    /**
     * Get all of the appointment for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\HasMany
     */
    public function appointment(): HasMany
    {
        return $this->hasMany(Appointment::class, 'user_id', 'staff_id');
    }

    /**
     * Get all of the order for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\HasMany
     */
    public function order(): HasMany
    {
        return $this->hasMany(Order::class, 'user_id', 'staff_id');
    }

    /**
     * Get all of the open service for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\HasMany
     */
    public function openServices()
    {
        return $this->belongsToMany(Service::class, 'therapist_open_services', 'user_id', 'services_id')
            ->using(TherapistOpenService::class);
    }

    /**
     * Get all of the open time for the User
     *
     * @return \Illuminate\Database\Eloquent\Relations\BelongsToMany
     */
    public function openTimes()
    {
        return $this->belongsToMany(Calendar::class, 'therapist_open_times', 'user_id', 'start_dt')
            ->using(TherapistOpenTime::class);
    }
}
